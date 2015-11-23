﻿namespace FSharpPlus

open FsControl

type OptionT<'``monad<option<'t>>``> = OptionT of '``monad<option<'t>>``

[<RequireQualifiedAccess>]
module OptionT =
    let run   (OptionT m) = m : '``Monad<option<'T>>``
    let inline map  (f:'T->'U) (OptionT m : OptionT<'``Monad<option<'T>``>)                                            = OptionT (Map.Invoke (Option.map f) m) : OptionT<'``Monad<option<'U>``>
    let inline bind (f:'T-> OptionT<'``Monad<option<'U>``>) (OptionT m : OptionT<'``Monad<option<'T>``>)               = (OptionT <| (m  >>= (fun maybe_value -> match maybe_value with Some value -> run (f value) | _ -> result None)))
    let inline apply (OptionT f : OptionT<'``Monad<option<('T -> 'U)>``>) (OptionT x : OptionT<'``Monad<option<'T>``>) = OptionT (Map.Invoke Option.apply f <*> x)  : OptionT<'``Monad<option<'U>``>

type OptionT with
    static member inline Map    (x : OptionT<'``Monad<option<'T>``>, f : 'T->'U , impl:Map)                                                       = OptionT.map f x                                                                             : OptionT<'``Monad<option<'U>``>
    static member inline Return (output : OptionT<'``Monad<option<'T>``>, impl:Return)                                                            = OptionT << result << Some                                                                   : 'T -> OptionT<'``Monad<option<'T>``>
    static member inline Apply  (f : OptionT<'``Monad<option<('T -> 'U)>``>, x : OptionT<'``Monad<option<'T>``>, output:OptionT<'r>, impl:Apply ) = OptionT.apply f x                                                                           : OptionT<'``Monad<option<'U>``>
    static member inline Bind   (x  : OptionT<'``Monad<option<'T>``>, f: 'T -> OptionT<'``Monad<option<'U>``>)                                    = OptionT.bind f x

    static member inline MZero (output: OptionT<'``MonadPlus<option<'T>``>, impl:MZero)                                                           = OptionT <| result None                                                                      : OptionT<'``MonadPlus<option<'T>``>
    static member inline MPlus (OptionT x, OptionT y, impl:MPlus)                                                                                 = OptionT <| (x  >>= (fun maybe_value -> match maybe_value with Some value -> x | _ -> y))    : OptionT<'``MonadPlus<option<'T>``>

    static member inline Lift (x:'``Monad<'T>``) = x |> (Map.FromMonad Some)           |> OptionT : OptionT<'``Monad<option<'T>>``>

    static member inline LiftAsync (x : Async<'T>) = Lift.Invoke (LiftAsync.Invoke x)

    static member inline ThrowError (x:'E) = x |> ThrowError.Invoke |> Lift.Invoke
    static member inline CatchError (m:OptionT<'``MonadError<'E1,'T>``>, h:'E1 -> OptionT<'``MonadError<'E2,'T>``>) = OptionT ((fun v h -> CatchError.Invoke v h) (OptionT.run m) (OptionT.run << h)) : OptionT<'``MonadError<'E2,'T>``>

    static member CallCC (f:(('T -> OptionT<Cont<'R,'U>>) -> _)) = OptionT(Cont.callCC <| fun c -> OptionT.run(f (OptionT << c << Some)))      :OptionT<Cont<'R,option<'T>>>

    static member get_Get() = Lift.Invoke State.get :OptionT<State<'S,_>>
    static member Put (x:'T) = x |> State.put |> Lift.Invoke :OptionT<_>

    static member get_Ask() = Lift.Invoke Reader.ask :OptionT<Reader<'R,option<'R>>>
    static member Local (OptionT (m:Reader<'R2,'T>), f:'R1->'R2) = OptionT <| Reader.local f m

    static member inline Tell (w:'Monoid) = w |> Writer.tell |> Lift.Invoke :OptionT<_>
    static member inline Listen (m : OptionT<Writer<'Monoid, option<'T>>>) =
        let liftMaybe (m, w) = Option.map (fun x -> (x, w)) m
        OptionT (Writer.listen (OptionT.run m) >>= (result << liftMaybe))
    static member inline Pass m : OptionT<Writer<'Monoid, option<'T>>> = OptionT (OptionT.run m >>= option (result None) (Map.Invoke Some << Writer.pass << result))

