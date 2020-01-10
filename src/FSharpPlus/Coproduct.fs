﻿namespace FSharpPlus.Data

open FSharpPlus
open FSharpPlus.Control

type CoproductBase<'``functorL<'t>``,'``functorR<'t>``> (left: '``functorL<'t>``, right: '``functorR<'t>``, isLeft: bool) =
    let (left, right, isLeft)    = left, right, isLeft
    with member _.getContents () = left, right, isLeft

type Coproduct<[<EqualityConditionalOn>]'``functorL<'t>``,'``functorR<'t>``> (left: '``functorL<'t>``, right: '``functorR<'t>``, isLeft: bool) =
    inherit CoproductBase<'``functorL<'t>``,'``functorR<'t>``> (left, right, isLeft)
    override x.GetHashCode () = Unchecked.hash (x.getContents ())
    override x.Equals o =
        match o with
        | :? Coproduct<'``functorL<'t>``,'``functorR<'t>``> as y -> Unchecked.equals (x.getContents ()) (y.getContents ())
        | _ -> false

[<AutoOpen>]
module CoproductPrimitives =
    let InL x = Coproduct<'``functorL<'t>``,'``functorR<'t>``> (x, Unchecked.defaultof<'``functorR<'t>``>, true)
    let InR x = Coproduct<'``functorL<'t>``,'``functorR<'t>``> (Unchecked.defaultof<'``functorL<'t>``>, x, false)
    let (|InL|InR|) (x: Coproduct<'``functorL<'t>``,'``functorR<'t>``>) = let (l, r, isL) = x.getContents () in if isL then InL l else InR r


type CoproductBase<'``functorL<'t>``,'``functorR<'t>``> with
    static member inline Map (x: obj, f: 'T -> 'U) : Coproduct<'``FunctorL<'U>``,'``FunctorR<'U>``> =
        let (l, r, isL) =  (x :?> Coproduct<'``FunctorL<'T>``,'``FunctorR<'T>``>).getContents ()
        if isL then InL (Map.Invoke f l)
        else        InR (Map.Invoke f r)

type Coproduct<'``functorL<'t>``,'``functorR<'t>``> with
    static member inline Map (a: Coproduct<'``FunctorL<'T>``,'``FunctorR<'T>``>, f: 'T -> 'U) : Coproduct<'``FunctorL<'U>``,'``FunctorR<'U>``> =
        let (l, r, isL) =  a.getContents ()
        if isL then InL (Map.InvokeOnInstance f l)
        else        InR (Map.InvokeOnInstance f r)