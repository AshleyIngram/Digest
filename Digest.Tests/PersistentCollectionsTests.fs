module Digest.Tests.PersistentCollectionsTests

open System.IO
open Digest.PersistentCollections
open FsUnit.Xunit
open Xunit

[<Fact>]
let ``An item added to the dictionary is available to any new dictionaries with the same id``() =
    let dict1 = new PersistentDictionary<string, int>("testDictionary")
    dict1.set("foo", 1)
    let dict2 = new PersistentDictionary<string, int>("testDictionary")
    dict2.get("foo") |> should equal 1
    File.Delete("storage/testDictionary")