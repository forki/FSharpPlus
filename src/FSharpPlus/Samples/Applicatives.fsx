#nowarn "3186"
#r @"..\..\build\FsControl.Core.dll"
#r @"..\..\build\FSharpPlus.dll"
open FSharpPlus

let lst5n6  = map ((+) 4) [ 1;2 ]
let arr5n6  = map ((+) 4) [|1;2|]
let lst5n6' = (+) <!> [4] <*> [1;2]
let opt120  = (+) <!> Some 20 <*> tryParse "100"

open FSharpPlus.Builders
let opt121  = iI (+) (Some 21) (tryParse "100") Ii