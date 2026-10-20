require "dreams"

player = GetEntity("__player")
footstepsAudio = GetEntity(this.GameObject.Name .. "Footsteps").DreamAudio
creakingAudio = GetEntity(this.GameObject.Name .. "Creaking").DreamAudio

interacted = false
distanceToPlayer = 0
moveSpeed = 0.3

function start()
    this.SetChildVisible(false)
    
    if IsDayEven() or (this.GameObject.Name ~= "Rickshaw1" and ChartZone() ~= "yellow") then
        this.GameObject.SetActive(false)
        return
    end
end

function intervalUpdate()
    if not interacted then return end
    distanceToPlayer = (player.WorldPosition - this.GameObject.WorldPosition).length()
end

function update()
    if not interacted then return end

    if distanceToPlayer < 0.8 and not linked then
        linked = true
        DreamSystem.SetNextTransitionDream(dreams.NaturalWorld)
        DreamSystem.TransitionToDream()
    end

    this.MoveInDirection(this.Forward, moveSpeed)
end

function interact()
    interacted = true
    footstepsAudio.Play()
    creakingAudio.Play()
    this.SetChildVisible(true)
    this.PlayAnimation(0)
    this.LogGraphContribution(-1, -2)
end