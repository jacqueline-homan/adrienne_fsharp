
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

let printIndent (indent:int) =
    printf "%s" (String(' ', 4*indent))

let rec toXml (root:YamlNode) (indent:int) =
    printIndent indent

    match root with
        | :? YamlMappingNode as mapping ->
                    printfn "Mapping Node: ["

                    for entry in mapping.Children do
                        printIndent (indent + 1)
                        printfn "%s: " ((entry.Key :?> YamlScalarNode).Value)
                        toXml entry.Value (indent + 1)
                    done

                    printIndent indent 
                    printfn "]"

        | :? YamlScalarNode as scalar ->
                    printf "%s" scalar.Value
        | :? YamlSequenceNode as seq ->
                    printf "Sequence Node: "
                    ignore (Seq.iter (fun n -> toXml n (indent + 1)) (seq.Children))
        | _ -> printf "Unrecognized node: %s" (root.ToString())
    printfn ""

let parseDocument (yamlObject:YamlObject) (document:string) =
    match yamlObject with
        | Asset | EvtEvent | NwsPost -> 
            printfn "%s" (printYamlObject yamlObject)
            let yaml = new YamlStream()
            let r = new StringReader(document)

            yaml.Load(r)

            toXml(yaml.Documents.[0].RootNode) 0
        | _ -> ()

[<EntryPoint>]
let main args = 
    let backupyml = new StreamReader(args.[0])

    documents backupyml parseDocument
    0
