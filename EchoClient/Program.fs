// For more information see https://aka.ms/fsharp-console-apps

open System
open System.IO
open System.Net
open System.Net.Sockets
let clientLoop (client: TcpClient, read: StreamReader, write: StreamWriter) = 
    while true do
        let input = Console.ReadLine()
        match input with
        | "" | null ->
            read.Close()
            write.Close()
            client.Close()
            exit 0
        | _ ->
            write.WriteLine(input.Trim())
            let output = read.ReadLine()
            printfn $"%s{output}"

[<EntryPoint>]
let main(args) =
    if (args.Length <> 2) then
        printfn "Usage: client [addr] [port]"
        exit 1
    let addr = IPAddress.Parse(args[0])
    let port = args[1] |> int
    let client = new TcpClient()
    client.Connect(addr, port)
    let dataStream = client.GetStream()
    let read = new StreamReader(dataStream)
    let write = new StreamWriter(dataStream, AutoFlush=true)
    clientLoop(client, read, write)
    0
