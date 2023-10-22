require "dreams"

player = GetEntity("__player")
audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio
moveSpeed = 0.25
distanceToPlayer = 0

function start()
    if Random.OneIn(2) then
        this.GameObject.SetActive(false)
        return
    end

    this.PlayAnimation(0)
    audio.Play()
end

function intervalUpdate()
    distanceToPlayer = (player.WorldPosition - this.GameObject.WorldPosition).length()
    this.SnapToFloor()
end

function update()
    if distanceToPlayer < 0.3 and not linked then
        linked = true
        DreamSystem.SetNextTransitionDream(dreams.TempleDojo)
        DreamSystem.TransitionToDream()
    end
    
    this.MoveInDirection(this.GameObject.ForwardDirection.negated(), moveSpeed)
end

function interact()
    this.LogGraphContribution(3, 0)
end