#r @"..\..\FSharpPlus\bin\Release\FsControl.Core.dll"
#load @"..\Operators.fs"
#load @"..\Extensions.fs"
#load @"..\ZipList.fs"

open FSharpPlus

let seq1 = seq { 1..100000}
let seq2 = seq {10..100000}

let seq1_plus_seq2  = (+) <!> ZipList seq1 <*> ZipList seq2

open ApplicativeMath

let seq1_plus_seq2' = ZipList seq1 |+| ZipList seq2
let arrCombined     = 10 *| ZipList seq1 |+| ZipList seq2 |- 5
let asMonoid        = result "Hello " </mappend/> ZipList ["City"; "World"; "Sun"]

// try ZipList.run {the results}