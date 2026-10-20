require "dreams"

group = GetEntity(this.GameObject.Name .. "_Group")
single = GetEntity(this.GameObject.Name .. "_Single")
audio = GetEntity(this.GameObject.Name .. "_Audio").DreamAudio
player = GetEntity("__player")

state = "none"
interacted = false
moveSpeed = 0.4
following = false
met = false
number = 0
grabbed = false
shakeTime = 0
fleeing = false

function start()
    number = tonumber(string.sub(this.GameObject.Name, 11))
    if number == 1 and ChartZone() == "yellow" then
        state = "grab"
    elseif number == 1 and Random.OneIn(3) then
        state = "flee"
    elseif number == 2 or number == 3 then
        -- single, drop and follow player
        state = "drop"
        this.InteractionDistance = 5
    elseif Random.OneIn(3) then
        state = "group"
    else
        state = "single"
    end

    hide()

    local hidden = (number == 2 or number == 3) and not IsDayLinear(3, 1)
    hidden = hidden or (number > 3 and not IsDayLinear(3, 2))
    if IsDayEven() or hidden then
        this.GameObject.SetActive(false)
        return
    end
end

function intervalUpdate()
    if not met then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if state == "grab" and not grabbed and playerDistance < 0.5 then
        grabbed = true
        SetCanControlPlayer(false)
        this.Action
            .WaitUntil(Condition.WaitForSeconds(25))
            .Then(|| DreamSystem.EndDream())
            .ThenFinish()
    elseif state == "flee" and not fleeing and playerDistance < 0.5 then
        fleeing = true
        single.AnimatedObject.Stop()
        this.Action
            .WaitUntil(Condition.WaitForSeconds(2))
            .Then(|| this.GameObject.SetActive(false))
            .ThenFinish()
    end

    if not interacted then return end

    if playerDistance < 0.3 then
        interacted = false
        DreamSystem.SetNextTransitionDream(dreams.ViolenceDistrict)
        DreamSystem.TransitionToDream()
    end

    this.SnapToFloor()
end

function update()
    if grabbed then
        shakeTime = shakeTime + Unity.DeltaTime()
        local shake = (shakeTime % 2 < 1 and player.RightDirection) or player.RightDirection.negated()
        player.WorldPosition = player.WorldPosition + shake * (0.25 * Unity.DeltaTime())
        return
    end

    if fleeing then
        this.MoveInDirection(this.Up, 2)
        return
    end

    if not following then return end

    this.LookAtPlane(player.WorldPosition)
    this.MoveTowards(player.WorldPosition, moveSpeed)
end

function interact()
    met = true

    if number > 1 then
        interacted = true
    end

    this.LogGraphContribution(-3, -6)

    show()
    audio.Play()

    if state == "drop" then
        single.AnimatedObject.Stop()
        this.Action
            .Do(|| SetCanControlPlayer(false))
            .Then(|| movePlayerInFront())
            .ThenWaitUntil(Condition.WaitForSeconds(7.5))
            .Then(|| SetCanControlPlayer(true))
            .Then(|| dropAndFollow())
            .ThenFinish()
    end
end

function dropAndFollow()
    single.WorldPosition = single.WorldPosition - Unity.Vector3(0, 0.375, 0)
    following = true
end

function movePlayerInFront()
    local inFront = this.GameObject.WorldPosition + (this.Forward * 1.75)
    player.WorldPosition = inFront
    player.LookAtPlane(this.GameObject.WorldPosition)
end

function hide()
    group.SetActive(false)
    single.SetActive(false)
end

function show()
    if state == "group" then
        group.SetActive(true)
        single.SetActive(false)
    else
        group.SetActive(false)
        single.SetActive(true)
    end
end