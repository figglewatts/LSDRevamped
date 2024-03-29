require "dreams"

audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio
player = GetEntity("__player")

active = false
linked = false
direction = nil
moveSpeed = 1
distanceToPlayer = 0

function start()
    if Random.OneIn(2) then
        -- set us tall
        this.StretchShrink(2)
    end

    -- choose walking direction
    local choice = Random.IntMax(4)
    if choice == 0 then
        direction = Unity.Vector3(1, 0, 0)
    elseif choice == 1 then
        direction = Unity.Vector3(-1, 0, 0)
    elseif choice == 2 then
        direction = Unity.Vector3(0, 0, 1)
    else
        direction = Unity.Vector3(0, 0, -1)
    end
    local targetPos = this.GameObject.WorldPosition - direction
    this.LookAt(targetPos)

    this.SetChildVisible(false)
    
    if not IsDayEven() or Random.OneIn(2) then
        this.GameObject.SetActive(false)
        return
    end
end

function intervalUpdate()
    distanceToPlayer = (player.WorldPosition - this.GameObject.WorldPosition).length()
end

function update()
    if not active or linked then return end

    if distanceToPlayer < 0.3 and not linked then
        linked = true
        DreamSystem.SetNextTransitionDream(dreams.PitAndTemple)
        DreamSystem.TransitionToDream()
    end

    this.MoveInDirection(direction, moveSpeed)
end

function interact()
    this.LogGraphContribution(0, 5)
    this.SetChildVisible(true)
    audio.Play()

    this.Action
        .Do(|| this.PlayAnimation(0))
        .ThenWaitUntil(this.WaitForAnimation(0))
        .Then(|| this.PlayAnimation(1))
        .Then(function() active = true end)
        .ThenFinish()
end