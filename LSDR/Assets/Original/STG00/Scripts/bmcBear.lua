require "dreams"

moveSpeed = 0.25

player = GetEntity("__player")
targetA = GetEntity("BearTargetA")
targetB = GetEntity("BearTargetB")
targetC = GetEntity("BearTargetC")
targetD = GetEntity("BearTargetD")
audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio

linked = false
finishedRotating = false
isTvBear = false
distanceToPlayer = 0

function start()
    isTvBear = this.GameObject.Name == "TVBear"
    if not IsDayEven() or (not isTvBear and Random.OneIn(2)) then
        this.GameObject.SetActive(false)
        return
    end
end

function intervalUpdate()
    distanceToPlayer = (player.WorldPosition - this.GameObject.WorldPosition).length()
end

function update()
    if linked then return end

    if distanceToPlayer < 0.3 and not linked then
        linked = true
        DreamSystem.SetNextTransitionDream(dreams.Happytown)
        DreamSystem.TransitionToDream()
    end

    this.SnapToFloor(0.3)
end

function interact()
    local target1 = (isTvBear and targetB) or targetA
    local target2 = targetC
    local target3 = targetD

    audio.Play()

    DreamSystem.LogGraphContributionFromEntity(9, 0)

    this.Action
        .Do(|| this.PlayAnimation(0))
        .ThenWaitUntil(this.WaitForAnimation(0))
        .Then(|| this.PlayAnimation(1))
        .Then(|| this.MoveTowards(target1.WorldPosition, moveSpeed))
        .Until(Condition.WaitForLinearMove(this.GameObject, target1.WorldPosition))
        .Then(function() finishedRotating = this.LookTowards(target2.WorldPosition, 25) end)
        .Until(Condition.Custom(|| finishedRotating))
        .Then(|| this.MoveTowards(target2.WorldPosition, moveSpeed))
        .Until(Condition.WaitForLinearMove(this.GameObject, target2.WorldPosition))
        .Then(function() finishedRotating = this.LookTowards(target3.WorldPosition, 25) end)
        .Until(Condition.Custom(|| finishedRotating))
        .Then(|| this.MoveTowards(target3.WorldPosition, moveSpeed))
        .Until(Condition.WaitForLinearMove(this.GameObject, target3.WorldPosition))
        .ThenFinish()
end