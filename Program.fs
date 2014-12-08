
// NOTE: If warnings appear, you may need to retarget this project to .NET 4.0. Show the Solution
// Pad, right-click on the project node, choose 'Options --> Build --> General' and change the target
// framework to .NET 4.0 or .NET 4.5.

module Adrienne.Main

open System
open System.IO

type YamlObject = | Asset | NwsPost | EvtEvent | Outlander

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
    let isObject = line.StartsWith "--- !ruby/object"
    match isObject with 
        | false -> None
        | true -> Some (yamlObjectYpe(line))

[<EntryPoint>]
let main args = 
    let backupyml = new StreamReader(args.[0])
    while not backupyml.EndOfStream do
        let line = backupyml.ReadLine()
        //if classObject line then 
        //    printfn "%s" line
        match classObject line with 
            | None -> ()
            | Some yamlObject -> printfn "%s" (printYamlObject(yamlObject))
            
    done
    0

