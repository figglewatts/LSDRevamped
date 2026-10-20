require "dreams"

audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio
player = GetEntity("__player")

active = false
linked = false
direction = nil
moveSpeed = 1
distanceToPlayer = 0

greenLine = { Statue6 = true, Statue7 = true, Statue8 = true, Statue9 = true }

function start()
    if Random.OneIn(6) then
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
    
    -- a line of them stands behind the first in the green part of the chart
    local inLine = greenLine[this.GameObject.Name]
    if inLine and ChartZone() ~= "green" then
        this.GameObject.SetActive(false)
        return
    end

    if not inLine and this.GameObject.Name ~= "Statue1" and IsDayEven() and ChartZone() ~= "green" then
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
    this.LogGraphContribution(0, -4)
    this.SetChildVisible(true)
    audio.Play()
    this.PlayAnimation(0)
    active = true
end