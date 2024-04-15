open System
open System.IO
open System.Net
open System.Net.Sockets

let rec echoLoop(client: TcpClient, read: StreamReader, write: StreamWriter) = async {
    let line = read.ReadLine()
    match line with
    | "" | null ->
        read.Close()
        write.Close()
        client.Close()
    | _ ->
        printfn $"%s{line}"
        write.WriteLine(line)
        return! echoLoop(client, read, write)
}

let listenerLoop (listener: TcpListener) = async {
    while true do
        let client = listener.AcceptTcpClient()
        printfn "Got Connection"
        let dataStream = client.GetStream()
        let read = new StreamReader(dataStream)
        let write = new StreamWriter(dataStream, AutoFlush=true)
        Async.Start(echoLoop(client, read, write))
}


[<EntryPoint>]
let main(args) =
    if (args.Length <> 1) then
        printfn "Usage: server [port]"
        exit 1
    let addr = IPAddress.Any
    let port = args[0] |> int
    let listener = new TcpListener(addr, port)
    listener.Start()
    Async.RunSynchronously(listenerLoop(listener))
    0
