require "dreams"

player = GetEntity("__player")
target1 = GetEntity("RunningBabyTarget1").WorldPosition
target2 = GetEntity("RunningBabyTarget2").WorldPosition
target3 = GetEntity("RunningBabyTarget3").WorldPosition
audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio

interacted = false
moveSpeed = 1.25
runTime = 0

function start()
    -- more of them run about in the blue part of the chart
    if this.GameObject.Name ~= "RunningBaby" and ChartZone() ~= "blue" then
        this.GameObject.SetActive(false)
    end
end

function update()
    if not interacted then return end

    runTime = runTime + Unity.DeltaTime()
    if runTime > 15 then
        this.GameObject.SetActive(false)
    end
end

function intervalUpdate()
    if not interacted then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance < 0.3 then
        interacted = false
        DreamSystem.SetNextTransitionDream(dreams.MonumentPark)
        DreamSystem.TransitionToDream()
    end
end

function interact()
    this.LogGraphContribution(4, 2)
    if interacted then return end
    interacted = true

    this.PlayAnimation(1)
    audio.Play()

    this.Action
        .Do(|| this.LookAtPlane(target1))
        .Then(|| this.MoveTowards(target1, moveSpeed))
        .Until(Condition.WaitForLinearMove(this.GameObject, target1))
        .Then(|| this.LookAtPlane(target2))
        .Then(|| this.MoveTowards(target2, moveSpeed))
        .Until(Condition.WaitForLinearMove(this.GameObject, target2))
        .Then(|| this.LookAtPlane(target3))
        .Then(|| this.MoveTowards(target3, moveSpeed))
        .Until(Condition.WaitForLinearMove(this.GameObject, target3))
        .Then(|| this.PlayAnimation(2))
        .ThenFinish()
end