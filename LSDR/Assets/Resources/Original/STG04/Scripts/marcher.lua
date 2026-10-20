require "dreams"

player = GetEntity("__player")
audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio

interacted = false
moveSpeed = 0.3
turns = { -90, 180, 90, 180 }
turnIndex = 1

function start()
    local lastBand = string.sub(this.GameObject.Name, 1, 8) == "Marcher3"
    if IsDayEven() == lastBand then
        this.GameObject.SetActive(false)
        return
    end

    audio.Play()
    this.PlayAnimation(0)
    this.Action
        .Do(|| this.MoveInDirection(this.Forward, moveSpeed))
        .Until(Condition.WaitForSeconds(10))
        .Then(turn)
        .ThenLoop()
end

function intervalUpdate()
    if not interacted then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance < 0.3 then
        interacted = false
        DreamSystem.SetNextTransitionDream(dreams.MonumentPark)
        DreamSystem.TransitionToDream()
    end

    this.SnapToFloor()
end

function interact()
    interacted = true
    this.LogGraphContribution(3, 3)
end

function turn()
    local rotation = this.GameObject.LocalRotation
    rotation.y = rotation.y + turns[turnIndex]
    this.GameObject.LocalRotation = rotation
    turnIndex = turnIndex % #turns + 1
end