
// NOTE: If warnings appear, you may need to retarget this project to .NET 4.0. Show the Solution
// Pad, right-click on the project node, choose 'Options --> Build --> General' and change the target
// framework to .NET 4.0 or .NET 4.5.

module Adrienne.Main

open System
open System.IO
open System.Text
open YamlDotNet.RepresentationModel

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


let documents (stream:StreamReader) (fn:YamlObject -> string -> unit) =
    let mutable (currentYamlObj:YamlObject option) = None
    let mutable document:string = ""
    
    while not stream.EndOfStream do
        let line = stream.ReadLine()

        match (classObject line) with 
            | Some nextYamlObject -> 
                match currentYamlObj with 
                    | Some y -> fn y document
                    | None -> ()
                currentYamlObj <- Some nextYamlObject
                document <- line + "\n"
            | None -> 
                document <- 
                    document + line + "\n"
    done

    match currentYamlObj with
        | Some y -> fn y document
        | None -> ()

let rec toXml (root:YamlNode) =
    match root with
        | :? YamlMappingNode as mapping ->
                    printfn "Mapping Node"

                    for entry in mapping.Children do
                        printf "Key: "
                        toXml entry.Key

                        printf "Value: "
                        toXml entry.Value
                    done

        | :? YamlScalarNode as scalar ->
                    printfn "scalar: Value: %s" scalar.Value
        | :? YamlSequenceNode as seq ->
                    printfn "Sequence Node"
                    ignore (Seq.iter toXml seq.Children)
        | _ -> printfn "Unrecognized node: %s" (root.ToString())

let parseDocument (yamlObject:YamlObject) (document:string) =
    match yamlObject with
        | Asset -> 
            printfn "%s" (printYamlObject yamlObject)
            let yaml = new YamlStream()
            let r = new StringReader(document)

            yaml.Load(r)

            toXml(yaml.Documents.[0].RootNode)
        | _ -> ()

[<EntryPoint>]
let main args = 
    let backupyml = new StreamReader(args.[0])

    documents backupyml parseDocument
    0
