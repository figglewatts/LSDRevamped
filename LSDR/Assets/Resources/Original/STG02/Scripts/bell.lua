audio = GetEntity("BellAudio").DreamAudio
videoClip = GetEntity("PetalsVideoClip").VideoClip
player = GetEntity("__player")

rings = 0

function start()
    DreamSystem.OnDreamTimeout(specialEvent)
end

function interact()
    this.LogGraphContribution(-5, 5)

    this.Action
        .Do(|| audio.Play())
        .Then(|| this.PlayAnimation(0))
        .ThenWaitUntil(this.WaitForAnimation(0))
        .Then(ring)
        .ThenLoop()
end

function ring()
    rings = rings + 1
    if rings == 35 and Random.OneIn(3) then
        DreamSystem.EndDream()
    end
end

function specialEvent()
    if not this.GameObject.Active then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance > 10 then
        return
    end

    videoClip.Play(Unity.ColorRGB(1, 0, 0))
end