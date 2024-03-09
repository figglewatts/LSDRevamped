test_table = {
    one="two",
    three= {
        four=5.678
    }
}

test_array = {
    "one", "two", "three", "four"
}

function start()
    if Persistence.HasValue("test_stored") then
        print(Persistence.RetrieveString("test_string"))
        print(Persistence.RetrieveNumber("test_float"))
        print(Persistence.RetrieveNumber("test_int"))
        print(json.parse(Persistence.RetrieveString("test_array")))
        print(json.parse(Persistence.RetrieveString("test_table")))
    else
        Persistence.StoreBoolean("test_stored", Lifetime.SaveFile, true)
        Persistence.StoreString("test_string", Lifetime.SaveFile, "Hello, world!")
        Persistence.StoreNumber("test_float", Lifetime.SaveFile, 1.234)
        Persistence.StoreNumber("test_int", Lifetime.SaveFile, 67)
        Persistence.StoreString("test_array", Lifetime.SaveFile, json.serialize(test_array))
        Persistence.StoreString("test_table", Lifetime.SaveFile, json.serialize(test_table))
        Persistence.StoreString("test_nonsaved", Lifetime.Dream, "test")
        DreamSystem.ForceSave()
    end
end

function update()
end