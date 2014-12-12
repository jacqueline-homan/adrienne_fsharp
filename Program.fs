
// NOTE: If warnings appear, you may need to retarget this project to .NET 4.0. Show the Solution
// Pad, right-click on the project node, choose 'Options --> Build --> General' and change the target
// framework to .NET 4.0 or .NET 4.5.

module Adrienne.Main

open System
open System.IO
open System.Text
open YamlDotNet.RepresentationModel
open System.Xml

type YamlObject = 
    | Asset
    | NwsPost
    | EvtEvent
    | Outlander

type OutputDocs = Collections.Generic.Dictionary<string,XmlDocument>

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

     
let documents (stream:StreamReader) 
              (outDocs:OutputDocs) 
              (fn:YamlObject -> string -> OutputDocs -> unit) =
    let mutable (currentYamlObj:YamlObject option) = None
    let mutable document:string = ""
    
    while not stream.EndOfStream do
        let line = stream.ReadLine()

        match (classObject line) with 
            | Some nextYamlObject -> 
                match currentYamlObj with 
                    | Some y -> fn y document outDocs
                    | None -> ()
                currentYamlObj <- Some nextYamlObject
                document <- line + "\n"
            | None -> 
                document <- 
                    document + line + "\n"
    done

    match currentYamlObj with
        | Some y -> fn y document outDocs
        | None -> ()

let mappingKey (entry:Collections.Generic.KeyValuePair<YamlNode,YamlNode>) : string =
    (entry.Key :?> YamlScalarNode).Value

let rec toXml (root:YamlNode) (xmlRoot:XmlNode) (xmlDocument:XmlDocument) =
    match root with
        | :? YamlMappingNode as mapping ->
                    for entry in mapping.Children do
                        let e = xmlDocument.CreateElement(mappingKey entry)
                        let r = xmlRoot.AppendChild(e)
                        toXml entry.Value r xmlDocument
                    done
                
        | :? YamlScalarNode as scalar ->
                    let v = scalar.Value
                    xmlRoot.InnerText <- v
        | :? YamlSequenceNode as seq ->
                    ignore (Seq.iter (fun n -> toXml n xmlRoot xmlDocument) (seq.Children))
        | _ -> printf "Unrecognized node: %s" (root.ToString())
    printfn ""

let createWriter (filename:string) : XmlWriter =
    let settings = new XmlWriterSettings()
    settings.Indent <- true
    XmlWriter.Create(filename + ".xml", settings)

let parseDocument (yamlObject:YamlObject) (document:string) (outDocs:OutputDocs) =
    match yamlObject with
        | Asset | EvtEvent | NwsPost -> 
            let xmlDocument = outDocs.[printYamlObject yamlObject]
            let root = xmlDocument.FirstChild.AppendChild(xmlDocument.CreateElement(printYamlObject yamlObject))
            let yaml = new YamlStream()
            let r = new StringReader(document)
            yaml.Load(r)
            toXml (yaml.Documents.[0].RootNode) root xmlDocument
        | _ -> ()

[<EntryPoint>]
let main args = 
    let backupyml = new StreamReader(args.[0])

    let outDocs = new OutputDocs()

    Seq.iter(
        fun t ->
                let d = new XmlDocument()
                ignore (d.AppendChild(d.CreateElement(t + "s")))
                outDocs.Add(t, d)
        ) (["Asset"; "NwsPost"; "EvtEvent";])
    
    documents backupyml outDocs parseDocument

    Seq.iter(
        fun t ->
                printfn "Writing %s.xml" t
                let out = outDocs.[t]
                use w = createWriter t
                out.WriteTo(w)
            )
        (["Asset"; "NwsPost"; "EvtEvent";])
    0
