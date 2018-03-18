﻿namespace FSharpPlus.Data

open FSharpPlus.Operators
open System.ComponentModel

/// The dual of a monoid, obtained by swapping the arguments of append.
[<Struct>]
type Dual<'t> = Dual of 't with
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    static member inline get_Zero () = Dual (getZero ())        : Dual<'T>
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    static member inline (+) (Dual x, Dual y) = Dual (plus y x) : Dual<'T>

/// Basic operations on Dual
[<RequireQualifiedAccess>]
module Dual = let run (Dual x) = x          : 'T

/// The monoid of endomorphisms under composition.
[<Struct; NoEquality; NoComparison>]
type Endo<'t> = Endo of ('t -> 't) with
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    static member get_Zero () = Endo id                : Endo<'T>
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    static member (+) (Endo f, Endo g) = Endo (f << g) : Endo<'T>

/// Basic operations on Endo
[<RequireQualifiedAccess>]
module Endo = let run (Endo f) = f : 'T -> 'T


/// Boolean monoid under conjunction.
[<Struct>]
type All = All of bool with
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    static member Zero = All true
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    static member (+) (All x, All y) = All (x && y)

/// Boolean monoid under disjunction.
[<Struct>]
type Any = Any of bool with
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    static member Zero = Any false
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    static member (+) (Any x, Any y) = Any (x || y)


/// <summary> The Const functor, defined as Const&lt;&#39;T, &#39;U&gt; where &#39;U is a phantom type. Useful for: Lens getters Its applicative instance plays a fundamental role in Lens.
/// <para/>   Useful for: Lens getters.
/// <para/>   Its applicative instance plays a fundamental role in Lens. </summary>
[<Struct>]
type Const<'t,'u> = Const of 't with

    // Monoid
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    static member inline get_Zero () = Const (getZero ()) : Const<'T,'U>
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    static member inline (+) (Const x : Const<'T,'U>, Const y : Const<'T,'U>) = Const (plus x y) : Const<'T,'U>

    // Functor
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    static member Map (Const x : Const<_,'T>, _:'T->'U) = Const x : Const<'C,'U>

    // Applicative
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    static member inline Return (_:'U) = Const (getZero ()) : Const<'T,'U>
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    static member inline (<*>) (Const f : Const<'C,'T->'U>, Const x : Const<'C,'T>) = Const (plus f x) : Const<'C,'U>

    // Contravariant
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    static member Contramap (Const x : Const<'C,'T>, _:'U->'T) = Const x     : Const<'C,'U>

    // Bifunctor
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    static member First     (Const x : Const<'T,'V>, f:'T->'U) = Const (f x) : Const<'U,'V>
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    static member Second    (Const x : Const<'T,'V>, _:'V->'W) = Const x     : Const<'T,'W>

/// Basic operations on Const
[<RequireQualifiedAccess>]
module Const =
    let run (Const t) = t


/// Option<'T> monoid returning the leftmost non-None value.
[<Struct>]
type First<'t> = First of Option<'t> with
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    static member get_Zero () = First None                                    : First<'t>
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    static member (+) (x, y) = match x, y with First None, r -> r | l, _ -> l : First<'t>
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    static member run (First a) = a                                           : 't option

/// Option<'T> monoid returning the rightmost non-None value.
[<Struct>]
type Last<'t> = Last of Option<'t> with
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    static member get_Zero () = Last None                                     : Last<'t>
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    static member (+) (x, y) = match x, y with l, Last None -> l | _, r -> r  : Last<'t>
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    static member run (Last a) = a                                            : 't option


/// Numeric wrapper for multiplication monoid (*, 1)
[<Struct>]
type Mult<'a> = Mult of 'a with
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    static member inline get_Zero () = Mult one
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    static member inline (+) (Mult (x:'n), Mult (y:'n)) = Mult (x * y)


/// Right-to-left composition of functors. The composition of applicative functors is always applicative, but the composition of monads is not always a monad.
[<Struct>]
type Compose<'``f<'g<'t>>``> = Compose of '``f<'g<'t>>`` with

    // Functor
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    static member inline Map (Compose x, f:'T->'U) = Compose (map (map f) x)

    // Applicative
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    static member inline Return (x:'T) = Compose (result (result x)) : Compose<'``F<'G<'T>``>
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    static member inline (<*>)  (Compose (f: '``F<'G<'T->'U>>``), Compose (x: '``F<'G<'T>>``)) = Compose ((<*>) <!> f <*> x: '``F<'G<'U>>``)

    // Alternative
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    static member inline get_Empty ()                  = Compose (getEmpty ()) : Compose<'``F<'G<'T>``>
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    static member inline (<|>) (Compose x, Compose y) = Compose (x <|> y)     : Compose<'``F<'G<'T>``>


/// Basic operations on Compose
[<RequireQualifiedAccess>]
module Compose =
    let run (Compose t) = t