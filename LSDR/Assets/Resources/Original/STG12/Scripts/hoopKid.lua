require "dreams"

player = GetEntity("__player")
audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio
targetA = GetEntity(this.GameObject.Name .. "TargetA").WorldPosition
targetB = GetEntity(this.GameObject.Name .. "TargetB").WorldPosition

interacted = false
moveSpeed = 0.3
walkTime = 0

function start()
    local huge = this.GameObject.Name ~= "HoopKid"
    if huge ~= (ChartZone() == "yellow") then
        this.GameObject.SetActive(false)
        return
    end

    if not huge and Random.OneIn(3) then
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

function update()
    if not interacted then return end

    walkTime = walkTime + Unity.DeltaTime()
    if walkTime > 120 then
        this.LookAtPlane(player.WorldPosition)
    end
end

function interact()
    this.LogGraphContribution(9, 0)
    if interacted then return end
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