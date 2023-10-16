require "dreams"

group = GetEntity(this.GameObject.Name .. "_Group")
single = GetEntity(this.GameObject.Name .. "_Single")
audio = GetEntity(this.GameObject.Name .. "_Audio").DreamAudio
player = GetEntity("__player")

state = "none"
interacted = false
moveSpeed = 0.4
following = false

function start()
    if IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end

    local randResult = Random.Float()
    if randResult < 0.1 then
        -- single, drop and follow player
        state = "drop"
        this.InteractionDistance = 5
    elseif randResult < 0.4 then
        -- group
        state = "group"
    else
        -- single
        state = "single"
    end

    hide()
end

function intervalUpdate()
    if not interacted then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance < 0.3 then
        interacted = false
        DreamSystem.SetNextTransitionDream(dreams.ViolenceDistrict)
        DreamSystem.TransitionToDream()
    end

    this.SnapToFloor()
end

function update()
    if not following then return end

    this.LookAtPlane(player.WorldPosition)
    this.MoveTowards(player.WorldPosition, moveSpeed)
end

function interact()
    if state == "drop" then
        -- we can only link if we're dropping down and following
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