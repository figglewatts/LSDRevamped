require "dreams"

player = GetEntity("__player")
warpPoints = {
    GetEntity("GirlWarp1"),
    GetEntity("GirlWarp2"),
    GetEntity("GirlWarp3"),
    GetEntity("GirlWarp4"),
    GetEntity("GirlWarp5")
}

function start()
    if not IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end
end

function intervalUpdate()
    if not interacted then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance < 0.5 then
        if Random.OneIn(4) then
            scarePlayer()
        else
            warp()
        end
    end
end

function interact()
    interacted = true
    this.LogGraphContribution(4, -5)
end

function warp()
    -- warp to a new location on the map
    local warpTarget = warpPoints[Random.IntMinMax(1, #warpPoints)].WorldPosition
    this.GameObject.WorldPosition = warpTarget
end

function playerTransition()
    DreamSystem.SetNextTransitionDream(dreams.Happytown)
    DreamSystem.SetNextTransitionColor(Colors.Red)
    DreamSystem.TransitionToDream()
end

function scarePlayer()
    interacted = false
    local toPlayer = player.WorldPosition - this.GameObject.WorldPosition
    this.GameObject.WorldPosition = player.WorldPosition - (toPlayer * 2.5)
    this.LookAtPlane(player.WorldPosition)
    this.Action
        .Do(|| this.PlayAnimation(0))
        .Then(|| SetCanControlPlayer(false))
        .ThenWaitUntil(this.WaitForAnimation(0, -0.2))
        .Then(|| this.StopAnimation())
        .Then(|| playerTransition())
        .ThenFinish()
end