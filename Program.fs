
// NOTE: If warnings appear, you may need to retarget this project to .NET 4.0. Show the Solution
// Pad, right-click on the project node, choose 'Options --> Build --> General' and change the target
// framework to .NET 4.0 or .NET 4.5.

module Adrienne.Main

open System
open System.IO
open YamlDotNet

type YamlDocument = string[]

type YamlObject = 
    | Asset
    | NwsPost
    | EvtEvent
    | Outlander

let printYamlObject(y:YamlObject):string = 
    match y with 
        | Asset -> "Asset"
        | NwsPost -> "NwsPost"
        | EvtEvent -> "EvtEvent"
        | Outlander -> "Outlander"

let yamlObjectYpe (line:string):YamlObject = 
    let item = (line.Split( [|':'|], 2)).[1].Trim()
    match item with
        | "Asset" -> Asset
        | "NwsPost" -> NwsPost
        | "EvtEvent" -> EvtEvent 
        | _     -> Outlander          // a match antyhing match

let classObject (line:string):YamlObject option = 
    match (line.StartsWith "--- !ruby/object") with 
        | false -> None
        | true -> Some (yamlObjectYpe(line))

let documents (stream:StreamReader) fn =
    let mutable (currentYamlObj:YamlObject option) = None
    let mutable document:string[] = [||]
    
    while not stream.EndOfStream do
        let line = stream.ReadLine().Trim()

        match (classObject line) with 
            | Some nextYamlObject -> 
                match currentYamlObj with 
                    | Some y -> fn y document
                    | None -> ()
                currentYamlObj <- Some nextYamlObject
                document <- [|line|]
            | None -> 
                document <- 
                    Collections.Array.append document [|line|]
    done

    match currentYamlObj with
        | Some y -> fn y document
        | None -> ()

[<EntryPoint>]
let main args = 
    let backupyml = new StreamReader(args.[0])

    documents backupyml (fun d l -> 
                            printfn "%s: %d lines" (printYamlObject d) (l.Length))
    0

