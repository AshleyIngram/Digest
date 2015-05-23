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
    File.Exists("storage/testDictionary_dict") |> should equal true
    File.Delete("storage/testDictionary_dict")

[<Fact>]
let ``An item added to a set is available to any set with the same id``() =
    let set1 = new PersistentSet<int>("testSet")
    set1.add(3)
    let set2 = new PersistentSet<int>("testSet")
    set2.contains(3) |> should equal true
    File.Exists("storage/testSet_set") |> should equal true
    File.Delete("storage/testSet_set")