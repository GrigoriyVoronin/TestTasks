package main

import (
"fmt"
)

func CalculateResult (f func(int) int, arg int, result chan <- int){
	result <- f(arg)
}

func Merge2Channels(f func(int) int, in1 <- chan int, in2 <- chan int, out chan <- int, n int) {
	go func() {
		for i := 0; i < n; i++ {
			result := make (chan int, 2)
			go CalculateResult (f, <- in1, result)
			go CalculateResult (f, <- in2, result)
			out <- <- result + <-result
		}
		close(out)
	}()
}

func fibonacci(n int) int {
	x, y := 0, 1
	for i := 0; i < n; i++ {
		x, y = y, x+y
	}

	return y
}

func main() {
	c := make(chan int, 10)
	a := make(chan int, 10)
	c <- fibonacci(cap(c))
	a <- fibonacci(cap(c))
	d:= make(chan int)
	Merge2Channels(fibonacci,a,c,d,1)
	for i := range d {
		fmt.Println(i)
	}
}