sirenAudio = GetEntity(this.GameObject.Name .. "SirenAudio").DreamAudio
engineAudio = GetEntity(this.GameObject.Name .. "EngineAudio").DreamAudio

blastingOff = false

function update()
    if not blastingOff then return end

    this.MoveInDirection(this.Up, 2)
end

function interact()
    DreamSystem.LogGraphContributionFromEntity(-5, -1)

    this.Action
        .Do(|| sirenAudio.Play())
        .ThenWaitUntil(Condition.WaitForSeconds(8))
        .Then(|| sirenAudio.Stop())
        .Then(|| this.PlayAnimation(0))
        .Then(|| engineAudio.Play())
        .Then(function() blastingOff = true end)
        .ThenWaitUntil(Condition.WaitForSeconds(20))
        .Then(|| this.GameObject.SetActive(false))
        .ThenFinish()
end