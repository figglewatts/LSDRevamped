player = GetEntity("__player")

function start()
    
end

function update()

end

function onTrigger5Down()
    local target1 = GetEntity("5FloorStairsTarget2")
    local target2 = GetEntity("5FloorStairsTarget3")
    local transitTrigger = GetEntity("4FloorStairsTriggerUpLua")

    player.Action
        .Do(|| player.PlayerMovement.OverrideMovement())
        .Then(|| transitTrigger.SetActive(false))
        .Then(|| player.PlayerMovement.RotateTowards(target1.WorldPosition))
        .ThenWaitUntil(Condition.PointingAt(player, target1.WorldPosition))
        .Then(|| player.PlayerMovement.StopMovementAndRotation())
        .Then(|| player.PlayerMovement.StartMovingForward())
        .Until(Condition.WaitForLinearMove(player, target1.WorldPosition))
        .Then(|| player.PlayerMovement.StopMovementAndRotation())

        .Then(|| player.PlayerMovement.OverrideMovement())
        .Then(|| player.PlayerMovement.RotateTowards(target2.WorldPosition))
        .ThenWaitUntil(Condition.PointingAt(player, target2.WorldPosition))
        .Then(|| player.PlayerMovement.StopMovementAndRotation())
        .Then(|| player.PlayerMovement.StartMovingForward())
        .Until(Condition.WaitForLinearMove(player, target2.WorldPosition))
        .Then(|| player.PlayerMovement.StopMovementAndRotation())
        .Then(|| transitTrigger.SetActive(true))
        .ThenFinish()
end

function onTrigger4Up()
    local target1 = GetEntity("5FloorStairsTarget2")
    local target2 = GetEntity("5FloorStairsTarget1")
    local transitTrigger = GetEntity("5FloorStairsTriggerDownLua")

    player.Action
        .Do(|| player.PlayerMovement.OverrideMovement())
        .Then(|| transitTrigger.SetActive(false))
        .Then(|| player.PlayerMovement.RotateTowards(target1.WorldPosition))
        .ThenWaitUntil(Condition.PointingAt(player, target1.WorldPosition))
        .Then(|| player.PlayerMovement.StopMovementAndRotation())
        .Then(|| player.PlayerMovement.StartMovingForward())
        .Until(Condition.WaitForLinearMove(player, target1.WorldPosition))
        .Then(|| player.PlayerMovement.StopMovementAndRotation())

        .Then(|| player.PlayerMovement.OverrideMovement())
        .Then(|| player.PlayerMovement.RotateTowards(target2.WorldPosition))
        .ThenWaitUntil(Condition.PointingAt(player, target2.WorldPosition))
        .Then(|| player.PlayerMovement.StopMovementAndRotation())
        .Then(|| player.PlayerMovement.StartMovingForward())
        .Until(Condition.WaitForLinearMove(player, target2.WorldPosition))
        .Then(|| player.PlayerMovement.StopMovementAndRotation())
        .Then(|| transitTrigger.SetActive(true))
        .ThenFinish()
end

function onTrigger4Down()
    doStairWalk(true, 4).ThenFinish()
end

function onTrigger3Up()
    doStairWalk(false, 3).ThenFinish()
end

function onTrigger3Down()
    doStairWalk(true, 3).ThenFinish()
end

function onTrigger2Up()
    doStairWalk(false, 2).ThenFinish()
end

function onTrigger2Down()
    doStairWalk(true, 2).ThenFinish()
end

function onTrigger1Up()
    doStairWalk(false, 1).ThenFinish()
end

function onTriggerExit()

end

function doStairWalk(down, floor)
    -- set up targets based on the floor we're on and whether we're going up or down
    local target1 = nil
    local target2 = nil
    local target3 = nil
    local transitTrigger = nil
    if down == true then
        target1 = GetEntity(floor .. "FloorStairsTarget2")
        target2 = GetEntity(floor .. "FloorStairsTarget3")
        target3 = GetEntity(floor .. "FloorStairsTarget4")
        transitTrigger = GetEntity((floor - 1) .. "FloorStairsTriggerUpLua")
    else
        target1 = GetEntity((floor + 1) .. "FloorStairsTarget3")
        target2 = GetEntity((floor + 1) .. "FloorStairsTarget2")
        target3 = GetEntity((floor + 1) .. "FloorStairsTarget1")
        transitTrigger = GetEntity((floor + 1) .. "FloorStairsTriggerDownLua")
    end
    
    -- set up the route to control the player down/up the stairs
    return player.Action
        .Do(|| player.PlayerMovement.OverrideMovement())
        .Then(|| transitTrigger.SetActive(false))
        .Then(|| player.PlayerMovement.RotateTowards(target1.WorldPosition))
        .ThenWaitUntil(Condition.PointingAt(player, target1.WorldPosition))
        .Then(|| player.PlayerMovement.StopMovementAndRotation())
        .Then(|| player.PlayerMovement.StartMovingForward())
        .Until(Condition.WaitForLinearMove(player, target1.WorldPosition))
        .Then(|| player.PlayerMovement.StopMovementAndRotation())

        .Then(|| player.PlayerMovement.OverrideMovement())
        .Then(|| player.PlayerMovement.RotateTowards(target2.WorldPosition))
        .ThenWaitUntil(Condition.PointingAt(player, target2.WorldPosition))
        .Then(|| player.PlayerMovement.StopMovementAndRotation())
        .Then(|| player.PlayerMovement.StartMovingForward())
        .Until(Condition.WaitForLinearMove(player, target2.WorldPosition))
        .Then(|| player.PlayerMovement.StopMovementAndRotation())

        .Then(|| player.PlayerMovement.OverrideMovement())
        .Then(|| player.PlayerMovement.RotateTowards(target3.WorldPosition))
        .ThenWaitUntil(Condition.PointingAt(player, target3.WorldPosition))
        .Then(|| player.PlayerMovement.StopMovementAndRotation())
        .Then(|| player.PlayerMovement.StartMovingForward())
        .Until(Condition.WaitForLinearMove(player, target3.WorldPosition))
        .Then(|| player.PlayerMovement.StopMovementAndRotation())
        .Then(|| transitTrigger.SetActive(true))
end