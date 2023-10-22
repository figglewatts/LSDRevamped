require "dreams"

audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio
player = GetEntity("__player")

active = false
linked = false
direction = nil
moveSpeed = 0.4
distanceToPlayer = 0

function start()
    if Random.OneIn(2) then
        this.GameObject.SetActive(false)
        return
    end

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
    this.LogGraphContribution(0, -4)
    this.SetChildVisible(true)
    audio.Play()
    this.PlayAnimation(0)
    active = true
end