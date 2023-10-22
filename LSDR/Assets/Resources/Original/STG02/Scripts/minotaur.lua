require "dreams"

audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio
player = GetEntity("__player")

linked = false
direction = nil
moveSpeed = 0.3
distanceToPlayer = 0

function start()
    if IsDayEven() or Random.OneIn(2) then
        this.GameObject.SetActive(false)
        return
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

    audio.Play()
    this.PlayAnimation(0)
end

function intervalUpdate()
    distanceToPlayer = (player.WorldPosition - this.GameObject.WorldPosition).length()
end

function update()
    if linked then return end

    if distanceToPlayer < 0.3 and not linked then
        linked = true
        DreamSystem.SetNextTransitionDream(dreams.Kyoto)
        DreamSystem.TransitionToDream()
    end

    this.MoveInDirection(direction, moveSpeed)
end

function interact()
    this.LogGraphContribution(-3, -1)
end