require "dreams"

player = GetEntity("__player")
audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio

interacted = false
moveSpeed = 0.75

function start()
    this.SetChildVisible(false)
    
    if IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end
end

function intervalUpdate()
    if not interacted then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance < 3.5 then
        interacted = false
        DreamSystem.SetNextTransitionDream(dreams.Void)
        DreamSystem.TransitionToDream()
    end
end

function update()
    if not interacted then return end

    this.MoveInDirection(this.Forward, moveSpeed)
end

function interact()
    this.LogGraphContribution(-5, 0)
    this.SetChildVisible(true)
    audio.Play()
    interacted = true
end