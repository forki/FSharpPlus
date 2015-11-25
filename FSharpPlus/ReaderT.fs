﻿namespace FSharpPlus

open FsControl

type ReaderT<'r,'``monad<'t>``> = ReaderT of ('r -> '``monad<'t>``)

[<RequireQualifiedAccess>]
module ReaderT =
    let  run (ReaderT x) = x    : 'R -> '``Monad<'T>``
    let inline map   (f:'T->'U) (ReaderT m : ReaderT<'R, '``Monad<'T>``>) = ReaderT (map f << m)                                                                 : ReaderT<'R, '``Monad<'U>``>
    let inline apply (ReaderT (f: _ -> '``Monad<'T -> 'U>``)) (ReaderT (x:_->'``Monad<'T>``)) = ReaderT (fun r -> f r <*> x r)                                          : ReaderT<'R, '``Monad<'U>``>
    let inline bind  (f:'T->_) (ReaderT (m:_->'``Monad<'T>``)) = ReaderT (fun r -> m r >>= (fun a -> run (f a) r))                                                      : ReaderT<'R, '``Monad<'U>``>

type ReaderT with
    static member inline Map    (x : ReaderT<'R, '``Monad<'T>``>, f:'T->'U, _:Map) = ReaderT.map f x                                                                    : ReaderT<'R, '``Monad<'U>``>
    static member inline Return (output : ReaderT<'R, '``Monad<'T>``>, _:Return) = fun (x:'T) -> ReaderT (fun _ -> result x)                                            : ReaderT<'R, '``Monad<'T>``> 
    static member inline Apply  (f:ReaderT<_,'``Monad<'T -> 'U>``>, x:ReaderT<_,'``Monad<'T>``>, output: ReaderT<'R, '``Monad<'U>``>, impl:Apply) = ReaderT.apply f x   : ReaderT<'R, '``Monad<'U>``>
    static member inline Bind   (x:ReaderT<_,'``Monad<'T>``>, f :'T->ReaderT<'R,'``Monad<'U>``>) = ReaderT.bind f x                                                     : ReaderT<'R, '``Monad<'U>``>
    
    static member inline MZero (output: ReaderT<'R, '``MonadPlus<'T>``>, impl:MZero) = ReaderT (fun _ -> getMZero())                                                : ReaderT<'R, '``MonadPlus<'T>``>
    static member inline MPlus (ReaderT m, ReaderT n, impl:MPlus)                    = ReaderT (fun r -> m r <|> n r)                                                   : ReaderT<'R, '``MonadPlus<'T>``>

    static member Lift m = ReaderT (fun _ -> m)                                             : ReaderT<'R,'``Monad<'T>``>

    static member inline CallCC (f : ('T -> ReaderT<'R, Cont<_,'U>>) -> _)                  : ReaderT<'R,'``MonadCont<'C,'T>``> =
        ReaderT (fun r -> callCC <| fun c -> ReaderT.run (f (fun a -> ReaderT <| fun _ -> c a)) r)
            
    static member inline get_Ask() = ReaderT result                                         : ReaderT<'R,'``Monad<'T>``>
    static member        Local (ReaderT m, f:_->'R2) = ReaderT(fun r -> m (f r))            : ReaderT<'R1,'``Monad<'T>``>

    static member inline LiftAsync (x: Async<'T>) = (lift (liftAsync x) : ReaderT<'R,'``MonadAsync<'T>``>)

    static member inline ThrowError (x:'E) = x |> throw |> lift : ReaderT<'R,'``MonadError<'E,'T>``>
    static member inline CatchError (m:ReaderT<'R,'``MonadError<'E1,'T>``>, h:'E1 -> _) = 
        ReaderT (fun s -> catch (ReaderT.run m s)   (fun e -> ReaderT.run (h e) s)) : ReaderT<'R,'``MonadError<'E2,'T>``>

    static member inline Tell   w           = w |> tell |> lift         : ReaderT<'R, '``MonadWriter<'Monoid,unit>``>
    static member inline Listen (ReaderT m) = ReaderT (fun w -> listen (m w))  : ReaderT<'R, '``MonadWriter<'Monoid,'T*'Monoid>``>
    static member inline Pass   (ReaderT m) = ReaderT (fun w -> pass   (m w))  : ReaderT<'R, '``MonadWriter<'Monoid,'T>``>   

    static member inline get_Get() = lift get         :ReaderT<'R, '``MonadState<'S, 'S>``>
    static member inline Put x     = x |> put |> lift :ReaderT<'R, '``MonadState<'S, unit>``>