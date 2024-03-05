audio = GetEntity("BellAudio").DreamAudio
videoClip = GetEntity("PetalsVideoClip").VideoClip
player = GetEntity("__player")

function start()
    DreamSystem.OnDreamTimeout(specialEvent)

    if Random.OneIn(2) then
        this.GameObject.SetActive(false)
        return
    end
end

function interact()
    this.LogGraphContribution(-5, 5)

    this.Action
        .Do(|| audio.Play())
        .Then(|| this.PlayAnimation(0))
        .ThenWaitUntil(this.WaitForAnimation(0))
        .ThenLoop()
end

function specialEvent()
    if not this.GameObject.Active then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance > 10 then
        return
    end

    videoClip.Play(Unity.ColorRGB(1, 0, 0))
end