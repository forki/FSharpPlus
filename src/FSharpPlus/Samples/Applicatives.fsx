#if INTERACTIVE
#r @"../bin/Release/net45/FSharpPlus.dll"
#else
module Samples.Applicatives
#endif
#nowarn "3186"

open FSharpPlus

let lst5n6  = map ((+) 4) [ 1;2 ]
let arr5n6  = map ((+) 4) [|1;2|]
let lst5n6' = (+) <!> [4] <*> [1;2]
let opt120  = (+) <!> Some 20 <*> tryParse "100"

open FSharpPlus.Builders
let opt121  = iI (+) (Some 21) (tryParse "100") Ii
let opt122  = iI tryDiv (tryParse "488") (trySqrt 16) Ji

// Composing applicatives

open FSharpPlus.Data

let res4 = (+) <!> Compose [Some 3] <*> Compose [Some 1]