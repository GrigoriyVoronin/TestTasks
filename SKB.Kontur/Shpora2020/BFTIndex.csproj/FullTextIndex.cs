using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BFTIndex.Models;

namespace BFTIndex
{
    public sealed class FullTextIndex : IFullTextIndex
    {
        private readonly DocumentsBuffer documents = new DocumentsBuffer();
        private readonly Dictionary<string, Regex> normalizationRegexs;
        private readonly string stopWordsRegexPattern;

        public FullTextIndex(IEnumerable<string> stopWords, IDictionary<char, char> normalizationTable)
        {
            Regex.CacheSize = 128;
            stopWordsRegexPattern = CreateStopWordsRegexPattern(stopWords);
            normalizationRegexs = CreateNormalizationRegex(normalizationTable);
        }

        public void AddOrUpdate(string documentId, string text) => documents.AddOrUpdate(documentId, text);

        public void Remove(string documentId) => documents.Remove(documentId);

        public MatchedDocument[] Search(string queryValue)
        {
            var query = new Query(queryValue, stopWordsRegexPattern, normalizationRegexs);

            if (query.Words.Length == 0)
                return new MatchedDocument[0];

            var documentsWithMatchedWords = documents
                .Where(doc => doc.IsAllWordsInDocument(query.Words));
            var documentsWithMatchedPhrases = documents
                .Where(doc => doc.IsAllWordsInDocument(query.Phrases));
            var notMatchedDocuments = documents
                .Where(doc => doc.IsNoOneWordInDocument(query.ForbiddenWords));

            return documentsWithMatchedWords
                .Intersect(documentsWithMatchedPhrases)
                .Intersect(notMatchedDocuments)
                .Select(doc => doc.GetMatchedDocument(documents, query.Words))
                .ToArray();
        }

        private static string CreateStopWordsRegexPattern(IEnumerable<string> stopWords)
        {
            var stopWordsRegexBuilder = new StringBuilder();
            stopWordsRegexBuilder.Append("(");
            foreach (var stopWord in stopWords)
                stopWordsRegexBuilder.AppendFormat($"{stopWord}|");
            stopWordsRegexBuilder.Remove(stopWordsRegexBuilder.Length - 1, 1);
            stopWordsRegexBuilder.Append(")");
            return stopWordsRegexBuilder.ToString();
        }

        private static Dictionary<string, Regex> CreateNormalizationRegex(IDictionary<char, char> normalizationTable) =>
            normalizationTable
                .Select(normalizationPair => $@"({normalizationPair.Key}|{normalizationPair.Value})")
                .ToDictionary(p => p,
                    p => new Regex(p, RegexOptions.Compiled));
    }

    public sealed class Query
    {
        private readonly Dictionary<string, Regex> normalizationRegex;

        public Query(string value, string stopWordsRegexPattern,
            Dictionary<string, Regex> normalizationRegex)
        {
            this.normalizationRegex = normalizationRegex;
            ParseQueryValue(value.ToLower(), stopWordsRegexPattern);
        }

        public string[] Words { get; private set; }
        public string[] Phrases { get; private set; }
        public string[] ForbiddenWords { get; private set; }


        private void ParseQueryValue(string value, string stopWordsRegexPattern)
        {
            var valueWithoutStopWords = value
                .RemoveStopWords(stopWordsRegexPattern)
                .RemoveExtraQuote();
            ForbiddenWords = GetForbiddenWords(valueWithoutStopWords);
            var valueWithoutForbiddenWords = valueWithoutStopWords
                .RemoveForbiddenWords()
                .RemoveAllNot();
            Words = GetWords(valueWithoutForbiddenWords);
            Phrases = GetPhrases(valueWithoutForbiddenWords, stopWordsRegexPattern);
        }

        private string[] GetWords(string value) =>
            CompiledRegexs.WordRegex
                .GetMatchesValues(value)
                .Select(Normalize)
                .ToArray();

        private string[] GetForbiddenWords(string value) =>
            CompiledRegexs.ForbiddenWordRegex
                .GetMatchesValues(value)
                .Select(v => v.Split().Last())
                .Select(Normalize)
                .ToArray();

        private string[] GetPhrases(string value, string stopWordsRegexPattern) =>
            CompiledRegexs.PhraseRegex
                .GetMatchesValues(value)
                .Select(phrase => phrase.Substring(1, phrase.Length - 2))
                .Select(Normalize)
                .Select(phrase => phrase.AddStopWordsPattern(stopWordsRegexPattern))
                .ToArray();

        private string Normalize(string value) =>
            normalizationRegex
                .Aggregate(value, (currentWord, regexPatternPair)
                    => regexPatternPair.Value.Replace(currentWord, regexPatternPair.Key));
    }

    public sealed class DocumentsBuffer : IEnumerable<DocumentInfo>
    {
        private readonly Dictionary<string, DocumentInfo> documents = new Dictionary<string, DocumentInfo>();
        private Dictionary<string, int> wordsEntries = new Dictionary<string, int>();
        private Dictionary<string, double> wordsIdf = new Dictionary<string, double>();

        public IEnumerator<DocumentInfo> GetEnumerator() => documents.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public double GetDocumentWeight(DocumentInfo document, IEnumerable<string> words)
            => words.Sum(word => document.GetWordTf(word) * GetWordIdf(word));

        private double GetWordIdf(string word) =>
            wordsIdf.GetOrInitValue(word,
                () => (double) documents.Count / GetWordEntriesCount(word));

        private int GetWordEntriesCount(string word) =>
            wordsEntries.GetOrInitValue(word,
                () => documents.Values.Count(doc => doc.ContainsWord(word)));

        public void AddOrUpdate(string documentId, string text)
        {
            documents[documentId] = new DocumentInfo(documentId, text);
            ClearWordsBuffers();
        }

        public void Remove(string documentId)
        {
            documents.Remove(documentId);
            ClearWordsBuffers();
        }

        private void ClearWordsBuffers()
        {
            wordsEntries = new Dictionary<string, int>(wordsEntries.Count);
            wordsIdf = new Dictionary<string, double>(wordsIdf.Count);
        }
    }

    public sealed class DocumentInfo
    {
        private static readonly Dictionary<string, Regex> WordsRegex = new Dictionary<string, Regex>();
        private readonly Dictionary<string, int> wordsCount = new Dictionary<string, int>();
        private readonly Dictionary<string, double> wordsTf = new Dictionary<string, double>();

        public DocumentInfo(string id, string text)
        {
            Id = id;
            Text = text;
            Length = CompiledRegexs.WordRegex.Matches(text).Count;
        }

        public string Id { get; }
        public long Length { get; }
        public string Text { get; }

        public bool ContainsWord(string word) => GetWordCount(word) > 0;

        public MatchedDocument GetMatchedDocument(DocumentsBuffer documents, IEnumerable<string> words) =>
            new MatchedDocument(Id, documents.GetDocumentWeight(this, words));

        public double GetWordTf(string word) =>
            wordsTf.GetOrInitValue(word,
                () => (double) GetWordCount(word) / Length);

        public int GetWordCount(string word) =>
            wordsCount.GetOrInitValue(word,
                () => GetRegexForWord(word).Matches(Text).Count);

        private static Regex GetRegexForWord(string word) =>
            WordsRegex.GetOrInitValue(word,
                () => new Regex($@"\b{word}\b", RegexOptions.IgnoreCase));

        public bool IsNoOneWordInDocument(IEnumerable<string> words) => !words.Any(ContainsWord);

        public bool IsAllWordsInDocument(IEnumerable<string> words) =>
            words.All(ContainsWord);

        public override bool Equals(object obj) => obj is DocumentInfo document
                                                   && Equals(document);

        public bool Equals(DocumentInfo other) => Id == other.Id;

        public override int GetHashCode() => Id != null ? Id.GetHashCode() : 0;
    }

    public static class CompiledRegexs
    {
        public static readonly Regex WordRegex = new Regex(@"\w+", RegexOptions.Compiled);
        public static readonly Regex NotRegex = new Regex(@"not", RegexOptions.Compiled);
        public static readonly Regex ForbiddenWordRegex = new Regex(@"(not )+\w+", RegexOptions.Compiled);
        public static readonly Regex QuoteRegex = new Regex("\"", RegexOptions.Compiled);
        public static readonly Regex PhraseRegex = new Regex("\"[^\"]+\"", RegexOptions.Compiled);

        private static string lastStopWordsPattern;
        private static Regex stopWordsRegex;

        public static Regex GetStopWordsRegex(string stopWordsPattern)
        {
            if (lastStopWordsPattern == null || lastStopWordsPattern != stopWordsPattern)
            {
                lastStopWordsPattern = stopWordsPattern;
                stopWordsRegex = new Regex($@"\b{stopWordsPattern}\b", RegexOptions.Compiled);
            }

            return stopWordsRegex;
        }
    }

    public static class RegexExtensions
    {
        public static IEnumerable<string> GetMatchesValues(this Regex regex, string input) =>
            regex.Matches(input)
                .Cast<Match>()
                .Select(m => m.Value);

        public static string RemoveMatches(this Regex regex, string input) =>
            regex.Replace(input, "");
    }

    public static class DictionaryExtensions
    {
        public static TValue GetOrInitValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary,
            TKey key, Func<TValue> valueCalculator) =>
            !dictionary.ContainsKey(key) ? dictionary[key] = valueCalculator.Invoke() : dictionary[key];
    }

    public static class StringExtensions
    {
        public static string RemoveExtraQuote(this string value)
        {
            var quoteNumber = CompiledRegexs.QuoteRegex.Matches(value).Count;
            if (quoteNumber % 2 == 0)
                return value;

            var lastIndex = value.LastIndexOf('"');
            return value.Remove(lastIndex, 1);
        }

        public static string AddStopWordsPattern(this string phrase, string stopWordsRegexPattern)
        {
            var phraseWords = phrase.Split();
            var phraseBuilder = new StringBuilder();
            for (var i = 0; i < phraseWords.Length; i++)
            {
                phraseBuilder.Append(phraseWords[i]);
                if (i != phraseWords.Length - 1)
                    phraseBuilder.Append($@"( {stopWordsRegexPattern} |\W*)");
            }

            return phraseBuilder.ToString();
        }

        public static string RemoveStopWords(this string phrase, string stopWordsRegexPattern) =>
            CompiledRegexs.GetStopWordsRegex(stopWordsRegexPattern)
                .RemoveMatches(phrase);

        public static string RemoveAllNot(this string value) =>
            CompiledRegexs.NotRegex.RemoveMatches(value);

        public static string RemoveForbiddenWords(this string value) =>
            CompiledRegexs.ForbiddenWordRegex.RemoveMatches(value);
    }


    internal sealed class FullTextIndexFactory : IFullTextIndexFactory
    {
        public IFullTextIndex Create(string[] stopWords, Dictionary<char, char> normalizationTable)
            => new FullTextIndex(stopWords, normalizationTable);
    }
}