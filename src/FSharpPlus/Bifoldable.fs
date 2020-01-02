﻿namespace FSharpPlus.Internals

namespace FSharpPlus.Control

open System.Runtime.InteropServices
open FSharpPlus.Internals

type BifoldMap =
    inherit Default1

    static member        BifoldMap (x: Result<_,_>, f, g, _impl: BifoldMap) = match x with Ok x -> f x | Error x -> g x
    static member        BifoldMap (x: Choice<_,_>, f, g, _impl: BifoldMap) = match x with Choice1Of2 x -> f x | Choice2Of2 x -> g x
    static member inline BifoldMap ((x,y)         , f, g, _impl: BifoldMap) = Plus.Invoke (f x) (g y)
    
    static member inline Invoke (f: 'T1->'U) (g: 'T2->'U) (source: '``Bifoldable<T1,T2>``) : 'U =
        let inline call (a: ^a, b: ^b) = ((^a or ^b) : (static member BifoldMap : _*_*_*_ -> _) b,f,g,a)
        call (Unchecked.defaultof<BifoldMap>, source)
        
    static member inline InvokeOnInstance (f: 'T1->'U) (g: 'T2->'U) (source: '``Bifoldable<T1,T2>``) : 'U =
        (^``Bifoldable<T1,T2>`` : (static member BifoldMap : _*_*_ -> _) source, f, g)

type BifoldMap with
    static member inline BifoldMap (x: '``Bifoldable<'T1,'T2>``, f: _ -> 'b, g: _ -> 'b, [<Optional>]_impl: Default1) = BifoldMap.InvokeOnInstance f g x : 'b
    static member inline BifoldMap (_: '``Bifoldable<'T1,'T2>`` when '``Bifoldable<'T1,'T2>`` : null and '``Bifoldable<'T1,'T2>``: struct, _, _, _: Default1) = id

type Bifold =
    inherit Default1

    static member inline Bifold (x: Result<_,_>, f            , g             , z    , _impl: Bifold) = match x with Ok x -> f z x | Error x -> g z x
    static member inline Bifold (x: Choice<_,_>, f: 's->'a->'s, g : 's->'b->'s, z: 's, _impl: Bifold) = match x with Choice1Of2 x -> f z x | Choice2Of2 x -> g z x
    static member inline Bifold ((x,y)         , f: 's->'a->'s, g : 's->'a->'s, z: 's, _impl: Bifold) = g (f z x) y

    static member inline Invoke (f: 'S->'T1->'S) (g: 'S->'T2->'S) (z: 'S) (source: '``Bifoldable<'T1,'T2>``) : 'S =
        let inline call (a: ^a, b: ^b) = ((^a or ^b) : (static member Bifold : _*_*_*_*_ -> _) b,f,g,z,a)
        call (Unchecked.defaultof<Bifold>, source)

    static member inline InvokeOnInstance (f: 'S->'T1->'S) (g: 'S->'T2->'S) (z: 'S) (source: '``Bifoldable<'T1,'T2>``) : 'S =
      (^``Bifoldable<'T1,'T2>`` : (static member Bifold : _*_*_*_ -> _) source, f, g, z)
      
type Bifold with
    static member inline Bifold (x: '``Bifoldable<'T1,'T2>``, f, g, z, [<Optional>]_impl: Default1) = Bifold.InvokeOnInstance f g z x
    static member inline Bifold (_: '``Bifoldable<'T1,'T2>`` when '``Bifoldable<'T1,'T2>`` : null and '``Bifoldable<'T1,'T2>``: struct, _, _, _, _: Default1) = id

type BifoldBack =
    inherit Default1

    static member inline BifoldBack (x: Result<_,_>, f            , g             , z    , _impl: BifoldBack) = match x with Ok x -> f x z | Error x -> g x z
    static member inline BifoldBack (x: Choice<_,_>, f: 'a->'s->'s, g : 'b->'s->'s, z: 's, _impl: BifoldBack) = match x with Choice1Of2 x -> f x z | Choice2Of2 x -> g x z
    static member inline BifoldBack ((x,y)         , f: 'a->'s->'s, g : 'b->'s->'s, z: 's, _impl: BifoldBack) = (f x (g y z))

    static member inline Invoke (f: 'T1->'S->'S) (g: 'T2->'S->'S) (z: 'S) (source: '``Bifoldable<'T1,'T2>``) : 'S =
        let inline call (a: ^a, b: ^b) = ((^a or ^b) : (static member BifoldBack : _*_*_*_*_ -> _) b,f,g,z,a)
        call (Unchecked.defaultof<BifoldBack>, source)

    static member inline InvokeOnInstance (f: 'T1->'S->'S) (g: 'T2->'S->'S) (z: 'S) (source: '``Bifoldable<'T1,'T2>``) : 'S =
      (^``Bifoldable<'T1,'T2>`` : (static member BifoldBack : _*_*_*_ -> _) source, f, g, z)
      
type BifoldBack with
    static member inline BifoldBack (x: '``Bifoldable<'T1,'T2>``, f, g, z, [<Optional>]_impl: Default1) = BifoldBack.InvokeOnInstance f g z x
    static member inline BifoldBack (_: '``Bifoldable<'T1,'T2>`` when '``Bifoldable<'T1,'T2>`` : null and '``Bifoldable<'T1,'T2>``: struct, _, _, _, _: Default1) = id

type Bisum =
    inherit Default1

    static member        Bisum (x: Result<_,_>, _impl: Bisum) = match x with Ok x -> x | Error x -> x 
    static member        Bisum (x: Choice<_,_>, _impl: Bisum) = match x with Choice1Of2 x -> x | Choice2Of2 x -> x
    static member inline Bisum ((x,y)         , _impl: Bisum) = Plus.Invoke x y

    static member inline Invoke (source: '``Bifoldable<'T1,'T2>``) : 'U =
        let inline call (a: ^a, b: ^b) = ((^a or ^b) : (static member Bisum : _*_ -> _) b,a)
        call (Unchecked.defaultof<Bisum>, source)

    static member inline InvokeOnInstance (source: '``Bifoldable<'T1,'T2>``) : 'U =
        (^``Bifoldable<'T1,'T2>`` : (static member Bisum : _ -> _) source)

type Bisum with
    static member inline Bisum (x: '``Bifoldable<'T,'T>``, [<Optional>]_impl: Default2) = BifoldMap.InvokeOnInstance id id x : 'T
    static member inline Bisum (x: '``Bifoldable<'T,'T>``, [<Optional>]_impl: Default1) = Bisum.InvokeOnInstance x : 'T
    static member inline Bisum (_: '``Bifoldable<'T,'T>`` when '``Bifoldable<'T,'T>`` : null and '``Bifoldable<'T,'T>``: struct, _: Default1) = ()
