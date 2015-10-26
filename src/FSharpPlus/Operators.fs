namespace FSharpPlus

open FsControl.Core.TypeMethods

[<AutoOpenAttribute>]
module Operators =


    // Common combinators
    let inline flip f x y = f y x
    let inline konst k _ = k
    let inline (</) x = (|>) x
    let inline (/>) x = flip x
    let inline choice f g = function Choice2Of2 x -> f x | Choice1Of2 y -> g y
    let inline option n f = function None -> n | Some x -> f x
    let inline tuple2 a b             = a,b
    let inline tuple3 a b c           = a,b,c
    let inline tuple4 a b c d         = a,b,c,d
    let inline tuple5 a b c d e       = a,b,c,d,e
    let inline tuple6 a b c d e f     = a,b,c,d,e,f
    let inline tuple7 a b c d e f g   = a,b,c,d,e,f,g
    let inline tuple8 a b c d e f g h = a,b,c,d,e,f,g,h


    // BEGIN region copied from FsControl

    // Functor ----------------------------------------------------------------

    /// Lift a function into a Functor.
    let inline map    (f:'T->'U) (x:'Functor'T) :'Functor'U = Map.Invoke f x

    /// Lift a function into a Functor. Same as map.
    let inline (<!>)  (f:'T->'U) (x:'Functor'T) :'Functor'U = Map.Invoke f x

    /// Lift a function into a Functor. Same as map but with flipped arguments.
    let inline (|>>)  (x:'Functor'T) (f:'T->'U) :'Functor'U = Map.Invoke f x

    /// Lift an action into a Functor.
    let inline map_   (action :'T->unit) (source :'Functor'T) = Map_.Invoke action source :unit
   

    // Applicative ------------------------------------------------------------

    /// Lift a value into a Functor. Same as return in Computation Expressions.
    let inline result (x:'T): 'Functor'T = Return.Invoke x

    /// Apply a lifted argument to a lifted function.
    let inline (<*>) (x:'Applicative'T_'U) (y:'Applicative'T): 'Applicative'U = Apply.Invoke x y : 'Applicative'U

    /// Apply 2 lifted arguments to a lifted function.
    let inline liftA2 (f:'T->'U->'V) (a:'Applicative'T) (b:'Applicative'U) :'Applicative'V = f <!> a <*> b

    let inline (  *>) (x:'Applicative'a) :'Applicative'b->'Applicative'b = x |> liftA2 (fun   _ -> id)
    let inline (<*  ) (x:'Applicative'a) :'Applicative'b->'Applicative'a = x |> liftA2 (fun k _ -> k )
    let inline (<**>) (x:'Applicative'a): 'Applicative'a_'b->'Applicative'b = x |> liftA2 (|>)
    let inline optional (v:'Alternative'a) :'Alternative'Option'a = Some <!> v <|> result None


    // Monad -----------------------------------------------------------
    
    let inline (>>=) (x:'Monad'T) (f:'T->'Monad'U) :'Monad'U = Bind.Invoke x f
    let inline (=<<) (f:'T->'Monad'U) (x:'Monad'T) :'Monad'U = Bind.Invoke x f
    let inline (>=>) (f:'a->'Monad'b) (g:'b->'Monad'c) (x:'a) :'Monad'c = Bind.Invoke (f x) g
    let inline (<=<) (g:'b->'Monad'c) (f:'a->'Monad'b) (x:'a) :'Monad'c = Bind.Invoke (f x) g
    let inline join (x:'Monad'Monad'T) :'Monad'T = Join.Invoke x


    // Monoid -----------------------------------------------------------------

    let inline mempty() :'Monoid = Mempty.Invoke()
    let inline mappend (x:'Monoid) (y:'Monoid): 'Monoid = Mappend.Invoke x y
    let inline mconcat (x:seq<'Monoid>)       : 'Monoid = Mconcat.Invoke x


    // Alternative/Monadplus/Arrowplus ----------------------------------------

    let inline mzero() :'Functor'T = Mzero.Invoke()
    let inline (<|>) (x:'Functor'T) (y:'Functor'T) :'Functor'T = Mplus.Invoke x y
    let inline guard x: 'MonadPlus'unit = if x then Return.Invoke () else Mzero.Invoke()

   
    // Contravariant/Bifunctor/Profunctor -------------------------------------

    let inline contramap (f:'T->'U) (x:'Contravariant'U) :'Contravariant'T = Contramap.Invoke f x
    let inline bimap f g x = Bimap.Invoke x f g
    let inline first   f x = First.Invoke f x
    let inline second  f x = Second.Invoke f x
    let inline dimap f g x = Dimap.Invoke x f g
    let inline lmap f x = Lmap.Invoke f x
    let inline rmap f x = Rmap.Invoke f x


    // Arrows -----------------------------------------------------------------

    let inline catId()     = Id.Invoke()
    let inline (<<<<)  f g = Comp.Invoke f g
    let inline (>>>>)  g f = Comp.Invoke f g
    let inline arr     f   = Arr.Invoke f
    let inline arrFirst  f = ArrFirst.Invoke f
    let inline arrSecond f = ArrSecond.Invoke f
    let inline ( ****) f g = arrFirst f >>>> arrSecond g
    let inline (&&&&)  f g = arr (fun b -> (b,b)) >>>> f **** g
    let inline (||||)  f g = AcEither.Invoke f g
    let inline (++++)  f g = AcMerge.Invoke  f g
    let inline left    f   =  AcLeft.Invoke f
    let inline right   f   = AcRight.Invoke f
    let inline arrApply()  = ArrApply.Invoke()


    // Foldable

    let inline foldBack (folder:'T->'State->'State) (foldable:'Foldable'T) (state:'State) :'State = FoldBack.Invoke folder state foldable
    let inline fold     (folder:'State->'T->'State) (state:'State) (foldable:'Foldable'T) :'State = Fold.Invoke folder state foldable
    let inline foldMap (f:'T->'Monoid) (x:'Foldable'T) :'Monoid = FoldMap.Invoke f x
    let inline toList  value :'t list = ToList.Invoke  value
    let inline toArray value :'t []   = ToArray.Invoke value
    let inline exists     (predicate :'T->bool) (source:'Foldable'T)   = Exists.Invoke  predicate source  :bool
    let inline forall     (predicate :'T->bool) (source:'Foldable'T)   = Forall.Invoke  predicate source  :bool
    let inline find       (predicate :'T->bool) (source:'Foldable'T)   = Find.Invoke    predicate source  :'T
    let inline tryFind    (predicate :'T->bool) (source:'Foldable'T)   = TryFind.Invoke predicate source  :'T option
    let inline pick     (chooser:'T->'U option) (source:'Foldable'T)   = Pick.Invoke    chooser   source  :'U
    let inline tryPick  (chooser:'T->'U option) (source:'Foldable'T)   = TryPick.Invoke chooser   source  :'U option
    let inline filter (predicate:_->bool) (x:'Foldable'a) :'Foldable'a =  Filter.Invoke predicate x


    // Traversable

    let inline traverse (f:'T->'Applicative'U) (t:'Traversable'T) :'Applicative'Traversable'U = Traverse.Invoke f t
    let inline sequenceA (t:'Traversable'Applicative'T) :'Applicative'Traversable'T = SequenceA.Invoke t


    // Comonads

    let inline extract (x:'Comonad'T): 'T = Extract.Invoke x
    let inline extend (g:'Comonad'T->'U) (s:'Comonad'T): 'Comonad'U = Extend.Invoke g s
    let inline (=>>)  (s:'Comonad'T) (g:'Comonad'T->'U): 'Comonad'U = Extend.Invoke g s
    let inline duplicate x = Duplicate.Invoke x //'Comonad'T -> :'Comonad'Comonad'T  


    // Monad Transformers

    open FsControl.Core.Types

    let inline lift      (x:'Monad'T ) :'MonadTrans'Monad'T = Lift.Invoke x

    let inline liftAsync (x:Async<'T>) :'MonadAsync'T       = LiftAsync.Invoke x

    let inline callCC (f:('T->'MonadCont'U)->'MonadCont'T): 'MonadCont'T = CallCC.Invoke f
   
    /// <summary>Haskell signature: get    :: MonadState  s m => m s</summary>
    let inline get() :'ms = Get.Invoke()

    /// <summary>Haskell signature: put    :: MonadState  s m => s -> m ()</summary>
    let inline put (x:'s) :'m = Put.Invoke x

    /// <summary>Haskell signature: ask    :: MonadReader r m => m r</summary>
    let inline ask() :'mr = Ask.Invoke()
   
    /// <summary>Haskell signature: local  :: MonadReader r m => (r -> r) -> m a -> m a</summary>
    let inline local (f:'rr) (m:'ma) :'ma = Local.Invoke f m

    /// <summary>Haskell signature: tell   :: MonadWriter w m => w   -> m ()</summary>
    let inline tell (x:'w) :'m = Tell.Invoke x

    /// <summary>Haskell signature: listen :: MonadWriter w m => m a -> m (a,w)</summary>
    let inline listen (m:'ma) :'maw = Listen.Invoke m

    /// <summary>Haskell signature: pass   :: MonadWriter w m => m (a, w -> w) -> m a</summary>
    let inline pass (m:'maww) :'ma = Pass.Invoke m

    /// <summary>Haskell signature: throw :: MonadError e m => e -> m a</summary>
    let inline throw (x:'e) :'ma = ThrowError.Invoke x

    /// <summary> Pure version of catch. Executes a function when the value represents an exception.
    /// <para/>   Haskell signature: catch :: MonadError e m => m a -> (e -> m b) -> m b </summary>
    let inline catch (v:'ma) (h:'e->'mb) :'mb = CatchError.Invoke v h


    // Collection

    let inline item (n:int) (source:'Collection'T) : 'T            = Item.Invoke n source
    let inline skip (n:int) (source:'Collection'T) : 'Collection'T = Skip.Invoke n source
    let inline take (n:int) (source:'Collection'T) : 'Collection'T = Take.Invoke n source

    let inline fromList (source :list<'t>) = FromList.Invoke source
    let inline fromSeq  (source :seq<'t> ) = FromSeq.Invoke  source

    let inline groupBy    (projection:'T->'Key) (source:'Collection'T) : 'Collection'KeyX'Collection'T = GroupBy.Invoke projection source
    let inline groupAdjBy (projection:'T->'Key) (source:'Collection'T) : 'Collection'KeyX'Collection'T = GroupAdjBy.Invoke projection source


    let inline choose (chooser:'T->'U option)   (source:'Collection'T)        = Choose.Invoke chooser source    :'Collection'U

    let inline distinct                         (source:'Collection'T)        = Distinct.Invoke              source  :'Collection'T
    let inline distinctBy (projection:'T->'Key) (source:'Collection'T)        = DistinctBy.Invoke projection source  :'Collection'T
    
    let inline head                             (source:'Collection'T)        = Head.Invoke source    :'T
    let inline tryHead                          (source:'Collection'T)        = TryHead.Invoke source :'T option

    let inline intersperse      (sep:'T)        (source:'Collection'T)        = Intersperse.Invoke sep source        :'Collection'T

    let inline iter       (action:'T->unit)     (source:'Collection'T)        = map_ action         source   :unit
    let inline iteri (action:int->'T->unit)     (source:'Collection'T)        = Iteri.Invoke action source   :unit

    let inline length (source:'Collection'T)                                  = Length.Invoke source      :int

    let inline mapi    (mapping:int->'T->'U)    (source:'Collection'T)        = Mapi.Invoke mapping source             :'Collection'U
    
    let inline maxBy (projection:'T->'U) (source:'Collection'T)               = MaxBy.Invoke projection  source    :'T
    let inline minBy (projection:'T->'U) (source:'Collection'T)               = MinBy.Invoke projection  source    :'T

    let inline rev  (source:'Collection'T)                                    = Rev.Invoke source :'Collection'T
    let inline scan (folder:'State'->'T->'State) state (source:'Collection'T) = Scan.Invoke folder (state:'State) source :'Collection'State

    let inline sort                         (source:'Collection'T) :'Collection'T = Sort.Invoke source 
    let inline sortBy (projection:'T->'Key) (source:'Collection'T) :'Collection'T = SortBy.Invoke projection source
    let inline toSeq (source:'Collection'T) = ToSeq.Invoke source  :seq<'T>

    let inline zip (source1:'Collection'T1) (source2:'Collection'T2)          = Zip.Invoke source1 source2     :'Collection'T1'T2




    // Tuple
    
    let inline item1 tuple = Item1.Invoke tuple
    let inline item2 tuple = Item2.Invoke tuple
    let inline item3 tuple = Item3.Invoke tuple
    let inline item4 tuple = Item4.Invoke tuple
    let inline item5 tuple = Item5.Invoke tuple
    let inline item6 tuple = Item6.Invoke tuple
    let inline item7 tuple = Item7.Invoke tuple
    let inline item8 tuple = Item8.Invoke tuple

    let inline mapItem1 mapping tuple = MapItem1.Invoke mapping tuple
    let inline mapItem2 mapping tuple = MapItem2.Invoke mapping tuple
    let inline mapItem3 mapping tuple = MapItem3.Invoke mapping tuple
    let inline mapItem4 mapping tuple = MapItem4.Invoke mapping tuple
    let inline mapItem5 mapping tuple = MapItem5.Invoke mapping tuple
    let inline mapItem6 mapping tuple = MapItem6.Invoke mapping tuple
    let inline mapItem7 mapping tuple = MapItem7.Invoke mapping tuple
    let        mapItem8 mapping tuple = tuple.MapItem8(mapping)
    
    
    
    // Converter

    /// Convert using the explicit operator.
    let inline explicit    (value:'T) :'U = Explicit.Invoke value

    let inline fromBytesWithOptions (isLtEndian:bool) (startIndex:int) (value:byte[]) = FromBytes.Invoke isLtEndian startIndex value
    let inline fromBytes   (value:byte[]) = FromBytes.Invoke true 0 value
    let inline fromBytesBE (value:byte[]) = FromBytes.Invoke false 0 value

    let inline toBytes   value :byte[] = ToBytes.Invoke true value
    let inline toBytesBE value :byte[] = ToBytes.Invoke false value

    let inline toStringWithCulture (cultureInfo:System.Globalization.CultureInfo) value:string = ToString.Invoke cultureInfo value
    let inline toString  value:string  = ToString.Invoke System.Globalization.CultureInfo.InvariantCulture value  
     
    /// Converts to a value from its string representation.
    let inline parse    (value:string) = Parse.Invoke    value

    /// Converts to a value from its string representation. Returns None if the convertion doesn't succeed.
    let inline tryParse (value:string) = TryParse.Invoke value


    // Numerics

    /// Gets a value that represents the number 0 (zero).
    let inline zero() = Zero.Invoke()

    /// Gets a value that represents the number 1 (one).
    let inline one()  = One.Invoke()

    /// Divides one number by another, returns a tuple with the result and the remainder.
    let inline divRem (D:'T) (d:'T) :'T*'T = DivRem.Invoke D d

    /// Returns the smallest possible value.
    let inline minValue() = MinValue.Invoke()

    /// Returns the largest possible value.
    let inline maxValue() = MaxValue.Invoke()

    /// Converts from BigInteger to the inferred destination type.
    let inline fromBigInt  (x:bigint)    :'Num   = FromBigInt.Invoke x

    /// Converts to BigInteger.
    let inline toBigInt    (x:'Integral) :bigint = ToBigInt.Invoke x

    /// Gets the pi number.
    let inline pi() :'Floating = Pi.Invoke()

    /// Returns the additive inverse of the number.
    let inline negate  (x:'Num): 'Num = x |> TryNegate.Invoke |> function Choice1Of2 x -> x | Choice2Of2 e -> raise e

    /// Returns the additive inverse of the number.
    /// Works also for unsigned types (Throws an exception if there is no inverse).
    let inline negate'  (x:'Num): 'Num = x |> TryNegate'.Invoke |> function Choice1Of2 x -> x | Choice2Of2 e -> raise e

    /// Returns the additive inverse of the number.
    /// Works also for unsigned types (Returns none if there is no inverse).
    let inline tryNegate'  (x:'Num): 'Num option = TryNegate'.Invoke x |> function Choice1Of2 x -> Some x | Choice2Of2 e -> None

    /// Returns the subtraction between two numbers. Throws an error if the result is negative on unsigned types.
    let inline subtract (x:'Num) (y:'Num): 'Num = Subtract.Invoke x y

    /// Returns the subtraction between two numbers. Returns None if the result is negative on unsigned types.
    let inline trySubtract (x:'Num) (y:'Num): 'Num option = y |> TrySubtract.Invoke x |> function Choice1Of2 x -> Some x | Choice2Of2 e -> None

    /// Returns the division between two numbers. If the numbers are not divisible throws an error.
    let inline div (dividend:'Num) (divisor:'Num): 'Num = Divide.Invoke dividend divisor

    /// Returns the division between two numbers. Returns None if the numbers are not divisible.
    let inline tryDiv (dividend:'Num) (divisor:'Num): 'Num option = divisor |> TryDivide.Invoke dividend |> function Choice1Of2 x -> Some x | Choice2Of2 e -> None

    /// Returns the square root of a number of any type. Throws an exception if there is no square root.
    let inline sqrt x = x |> Sqrt.Invoke

    /// Returns the square root of a number of any type. Returns None if there is no square root.
    let inline trySqrt x = x |> TrySqrt.Invoke |> function Choice1Of2 x -> Some x | Choice2Of2 _ -> None

    /// Returns the square root of an integral number.
    let inline isqrt   (x:'Integral): 'Integral = x |> TrySqrtRem.Invoke |> function Choice1Of2 (x, _) -> x | Choice2Of2 e -> raise e

    /// Returns the square root of an integral number.
    let inline sqrtRem   (x:'Integral): 'Integral*'Integral = x |> TrySqrtRem.Invoke |> function Choice1Of2 x -> x | Choice2Of2 e -> raise e

    /// <summary> Returns a number which represents the sign.
    /// <para/>   Rule: signum x * abs x = x        </summary>
    let inline signum (x:'Num): 'Num = Signum.Invoke x

    /// <summary> Returns a number which represents the sign.
    ///           Works also for unsigned types. 
    /// <para/>   Rule: signum x * abs x = x        </summary>
    let inline signum' (x:'Num): 'Num = Signum'.Invoke x

    /// <summary> Gets the absolute value of a number.
    /// <para/>   Rule: signum x * abs x = x        </summary>
    let inline abs    (x:'Num): 'Num = Abs.Invoke x

    /// <summary> Gets the absolute value of a number.
    ///           Works also for unsigned types. 
    /// <para/>   Rule: signum x * abs x = x        </summary>
    let inline abs'   (x:'Num): 'Num = Abs'.Invoke x

    // END region copied from FsControl.



    // Additional functions

    /// Folds a Foldable of a Monoid, using mempty as initial state and mappend as folder.
    let inline mfold (x:'Foldable'Monoid): 'Monoid = foldMap id x

    /// Returns the sum of the elements in the Foldable.
    let inline sum (x:'Foldable'Num) : 'Num = fold (+) (LanguagePrimitives.GenericZero: 'Num) x

    /// Converts using the implicit operator. 
    let inline implicit (x : ^t) = ((^R or ^t) : (static member op_Implicit : ^t -> ^R) x) :^R



    /// <summary>Math Operators ready to use over Applicative Functors.</summary>
    module ApplicativeMath =

        let inline ( |+  ) (x :'Functor't)     (y :'t)             = map ((+)/> y) x :'Functor't
        let inline (  +| ) (x :'t)             (y :'Functor't)     = map ((+)   x) y :'Functor't
        let inline ( |+| ) (x :'Applicative't) (y :'Applicative't) = (+) <!> x <*> y :'Applicative't

        let inline ( |-  ) (x :'Functor't)     (y :'t)             = map ((-)/> y) x :'Functor't
        let inline (  -| ) (x :'t)             (y :'Functor't)     = map ((-)   x) y :'Functor't
        let inline ( |-| ) (x :'Applicative't) (y :'Applicative't) = (-) <!> x <*> y :'Applicative't

        let inline ( |*  ) (x :'Functor't)     (y :'t)             = map ((*)/> y) x :'Functor't
        let inline (  *| ) (x :'t)             (y :'Functor't)     = map ((*)   x) y :'Functor't
        let inline ( |*| ) (x :'Applicative't) (y :'Applicative't) = (*) <!> x <*> y :'Applicative't

        let inline ( |%  ) (x :'Functor't)     (y :'t)             = map ((%)/> y) x :'Functor't
        let inline (  %| ) (x :'t)             (y :'Functor't)     = map ((%)   x) y :'Functor't
        let inline ( |%| ) (x :'Applicative't) (y :'Applicative't) = (%) <!> x <*> y :'Applicative't

        let inline ( |/  ) (x :'Functor't)     (y :'t)             = map ((/)/> y) x :'Functor't
        let inline (  /| ) (x :'t)             (y :'Functor't)     = map ((/)   x) y :'Functor't
        let inline ( |/| ) (x :'Applicative't) (y :'Applicative't) = (/) <!> x <*> y :'Applicative't

    

    /// <summary>
    /// Generic numbers, functions and operators.
    /// By opening this module some common operators become restricted, like (+) to 'a->'a->'a 
    /// </summary>
    module GenericMath =

        open System.Numerics

        let inline fromIntegral   (x:'Integral) :'Num   = (fromBigInt << toBigInt) x

        module NumericLiteralG =
            let inline FromZero() = zero()
            let inline FromOne () = one()
            let inline FromInt32  (i:int   ) = FromInt32.Invoke i
            let inline FromInt64  (i:int64 ) = FromInt64.Invoke i
            let inline FromString (i:string) = fromBigInt <| BigInteger.Parse i

        let inline (+) (a:'Num) (b:'Num) :'Num = a + b
        let inline (-) (a:'Num) (b:'Num) :'Num = a - b
        let inline (*) (a:'Num) (b:'Num) :'Num = a * b

        let inline internal whenIntegral a = let _ = if false then toBigInt a else 0I in ()
 
        /// Integer division following the mathematical convention where the mod is always positive.
        let inline div' (a:'Integral) b :'Integral =
            whenIntegral a
            let (a, b) = if b < 0G then (-a, -b) else (a, b)
            (if a < 0G then (a - b + 1G) else a) / b
 
        /// Integer division. Same as (/) for Integral types.
        let inline div (a:'Integral) (b:'Integral) :'Integral = whenIntegral a; a / b

        /// Remainder of Integer division. Same as (%).
        let inline rem (a:'Integral) (b:'Integral) :'Integral = whenIntegral a; a % b
 
        /// Greatest Common Divisor
        let inline gcd x y :'Integral =
            let zero = zero()
            let rec loop a = function
                | b when b = zero -> a
                | b -> loop b (rem a b)
            match(x,y) with
            | t when t = (zero,zero) -> failwith "gcd 0 0 is undefined"
            | _                      -> loop (abs x) (abs y)