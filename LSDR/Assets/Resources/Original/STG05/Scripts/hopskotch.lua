require "dreams"

player = GetEntity("__player")
audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio

function start()
    this.PlayAnimation(0)
    
    if Random.OneIn(4) then
        this.GameObject.Scale = Unity.Vector3(1, 2, 1)
    end
    
    if not IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end
end

function intervalUpdate()
    if not interacted then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance < 0.3 then
        interacted = false
        DreamSystem.SetNextTransitionDream(dreams.Void)
        DreamSystem.TransitionToDream()
    end
end

function interact()
    interacted = true
    this.LogGraphContribution(7, 0)
    audio.Play()
end