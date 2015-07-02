module Digest.Tests.PersistentCollectionsTests

open System.IO
open Digest.PersistentCollections
open FsUnit.Xunit
open Xunit

[<Fact>]
let ``An item added to the dictionary is available to any new dictionaries with the same id``() =
    try
        let dict1 = new PersistentDictionary<string, int>("testDictionary")
        dict1.Set("foo", 1)
        let dict2 = new PersistentDictionary<string, int>("testDictionary")
        dict2.Get("foo") |> should equal 1
        File.Exists("storage/testDictionary_dict") |> should equal true
    finally
        File.Delete("storage/testDictionary_dict")

[<Fact>]
let ``An item added to a set is available to any set with the same id``() =
    try
        let set1 = new PersistentSet<int>("testSet")
        set1.Add(3)
        let set2 = new PersistentSet<int>("testSet")
        set2.Contains(3) |> should equal true
        File.Exists("storage/testSet_set") |> should equal true
    finally
        File.Delete("storage/testSet_set")

[<Fact>]
let ``An item added to a queue is available to any queue with the same id``() =
    try
        let set1 = new PersistentQueue<string>("testQueue")
        set1.Enqueue("foo")
        let set2 = new PersistentQueue<string>("testQueue")
        set2.Dequeue() |> Option.get |> should equal "foo"
        File.Exists("storage/testQueue_queue") |> should equal true
    finally
        File.Delete("storage/testQueue_queue")