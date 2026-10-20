sirenAudio = GetEntity(this.GameObject.Name .. "SirenAudio").DreamAudio
engineAudio = GetEntity(this.GameObject.Name .. "EngineAudio").DreamAudio

launched = false
blastingOff = false
flightTime = 0

function update()
    if not blastingOff then return end

    flightTime = flightTime + Unity.DeltaTime()
    local speed = 0.6
    if flightTime > 15 then
        speed = 5
        this.Rotate(Unity.Vector3(10, 0, 0))
    elseif flightTime > 5 then
        speed = 2.5
    end
    this.MoveInDirection(this.Up, speed)
end

function interact()
    this.LogGraphContribution(-5, -1)

    if launched then return end
    launched = true

    this.Action
        .Do(|| sirenAudio.Play())
        .ThenWaitUntil(Condition.WaitForSeconds(8))
        .Then(|| sirenAudio.Stop())
        .Then(|| this.PlayAnimation(0))
        .Then(|| engineAudio.Play())
        .Then(function() blastingOff = true end)
        .ThenWaitUntil(Condition.WaitForSeconds(60))
        .Then(|| this.GameObject.SetActive(false))
        .ThenFinish()
end