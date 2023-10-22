require "dreams"

player = GetEntity("__player")
audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio
targetA = GetEntity(this.GameObject.Name .. "TargetA").WorldPosition
targetB = GetEntity(this.GameObject.Name .. "TargetB").WorldPosition

interacted = false
moveSpeed = 0.25

function start()
    if Random.OneIn(3) then
        this.GameObject.Scale = Unity.Vector3(2, 2, 2)
    end

    this.SetChildVisible(false)
end

function intervalUpdate()
    if not interacted then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance < 0.6 then
        interacted = false
        DreamSystem.SetNextTransitionDream(dreams.Kyoto)
        DreamSystem.TransitionToDream()
    end
end

function interact()
    this.LogGraphContribution(9, 0)
    interacted = true
    this.SetChildVisible(true)
    audio.Play()

    this.Action
        .Do(|| this.LookAtPlane(targetA))
        .Then(|| this.MoveTowards(targetA, moveSpeed))
        .Until(Condition.WaitForLinearMove(this.GameObject, targetA))
        .Then(|| this.LookAtPlane(targetB))
        .Then(|| this.MoveTowards(targetB, moveSpeed))
        .Until(Condition.WaitForLinearMove(this.GameObject, targetB))
        .ThenLoop()
end