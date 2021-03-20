namespace PushV1.Parameters.Abstractions
{
    #region using
    using System.Globalization;
    #endregion

    public abstract class FloatParameter : Parameter<float>
    {
        static FloatParameter()
        {
            ParseValueFunc = x => float.Parse(x, CultureInfo.InvariantCulture);
        }

        protected FloatParameter(string value)
            : base(value)
        {
        }
    }
}