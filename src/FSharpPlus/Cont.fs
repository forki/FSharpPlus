﻿namespace FSharpPlus

open FsControl

/// <summary> Computation type: Computations which can be interrupted and resumed.
/// <para/>   Binding strategy: Binding a function to a monadic value creates a new continuation which uses the function as the continuation of the monadic computation.
/// <para/>   Useful for: Complex control structures, error handling, and creating co-routines.</summary>
type Cont<'r,'t> = Cont of (('t->'r)->'r)

[<RequireQualifiedAccess>]
module Cont =
    let run (Cont x) = x      : ('T->'R)->'R  
    
    /// (call-with-current-continuation) calls a function with the current continuation as its argument.
    let callCC (f:('T->Cont<'R,'U>)->_) = Cont (fun k -> run (f (fun a -> Cont(fun _ -> k a))) k)

    let map  (f:'T->_) (Cont x) = Cont (fun c -> x (c << f))                                        : Cont<'R,'U>
    let bind (f:'T->_) (Cont x) = Cont (fun k -> x (fun a -> run(f a) k))                           : Cont<'R,'U>
    let apply (Cont f) (Cont x) = Cont (fun k -> f (fun (f':'T->_) -> x (k << f')))                 : Cont<'R,'U>

type Cont with
    static member Map    (x:Cont<'R,'T>, f) = Cont.map f x              : Cont<'R,'U>
    static member Return n = Cont (fun k -> k n)                        : Cont<'R,'T>
    static member Bind   (x, f:'T->_) = Cont.bind f x                   : Cont<'R,'U>
    static member (<*>)  (f, x:Cont<'R,'T>) = Cont.apply f x            : Cont<'R,'U>
    static member CallCC (f:('T -> Cont<'R,'U>) -> _) = Cont.callCC f   : Cont<'R,'T>