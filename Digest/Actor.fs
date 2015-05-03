module Digest.Actor

type IRecievingActor<'a> =
    abstract member Post: 'a -> unit
    abstract member Cancel: unit -> unit

type ISendingActor<'a> =
    abstract member AddChild: IRecievingActor<'a> -> unit

type State<'a, 'b, 'c> = { self: IRecievingActor<'a>; children: IRecievingActor<'b> list; value: 'c }

type ActorMessage<'a, 'b> =
        | Value of 'a
        | AddChild of IRecievingActor<'b>
        | Cancelled

type Actor<'a, 'b, 'c>(processingFunction, initialValue: 'c) as this =
    let rec mailbox = MailboxProcessor.Start(fun inbox ->
        let rec loop(state) = 
            async {
                let! message = inbox.Receive()
                
                match message with
                | Cancelled -> return 0
                | AddChild c -> 
                    let newState = { self = state.self; value = state.value; children = (c :: state.children) }
                    return! loop(newState)
                | Value v -> 
                    let! newState = processingFunction(v, state)
                    return! loop(newState) 
            }

        let initialState = { children = []; self = this; value = initialValue }
        loop(initialState) |> Async.Ignore
    )

    member this.Post(v) = (this :> IRecievingActor<'a>).Post(v)
    member this.Cancel = (this :> IRecievingActor<'a>).Cancel
    member this.AddChild(c) = (this :> ISendingActor<'b>).AddChild(c)

    interface IRecievingActor<'a> with
        member this.Cancel() = mailbox.Post(Cancelled)
        member this.Post(v) = mailbox.Post(Value v)
    
    interface ISendingActor<'b> with
        member this.AddChild(c) = mailbox.Post(AddChild c)