﻿#if INTERACTIVE
#r @"../bin/Release/net45/FSharpPlus.dll"
#else
module Samples.Fold
#endif
#nowarn "3186"

open FSharpPlus
open FSharpPlus.Lens

let r1 = over both length ("hello","world")
// val it : int * int = (5, 5)

let r2 = ("hello","world")^.both
// val it : string = "helloworld"

let r3 = anyOf both ((=)'x') ('x','y')
// val it : bool = true

let r4 = (1,2)^..both
//val it : int list = [1; 2]