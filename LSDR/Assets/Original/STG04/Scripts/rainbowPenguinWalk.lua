require "dreams"

player = GetEntity("__player")
target1 = GetEntity("RainbowPenguinTarget1")
target2 = GetEntity("RainbowPenguinTarget2")
target3 = GetEntity("RainbowPenguinTarget3")

interacted = false
moveSpeed = 0.3

actionsFinished = false

function start()
    if IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end

    if Random.OneIn(4) then
        this.GameObject.Scale = Unity.Vector3(1, 1, 1)
    end

    this.SetChildVisible(false)
end

function intervalUpdate()
    if not interacted then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance < 0.3 then
        interacted = false
        DreamSystem.SetNextTransitionDream(dreams.Happytown)
        DreamSystem.TransitionToDream()
    end
end

function update()
    if not actionsFinished then return end

    this.MoveInDirection(this.Forward, moveSpeed)
end

function interact()
    this.SetChildVisible(true)
    interacted = true
    this.LogGraphContribution(1, -1)
    this.PlayAnimation(0)

    this.Action
        .Do(|| this.LookAtPlane(target1.WorldPosition))
        .Then(|| this.MoveTowards(target1.WorldPosition, moveSpeed))
        .Until(Condition.WaitForLinearMove(this.GameObject, target1.WorldPosition))
        .Then(|| this.LookAtPlane(target2.WorldPosition))
        .Then(|| this.MoveTowards(target2.WorldPosition, moveSpeed))
        .Until(Condition.WaitForLinearMove(this.GameObject, target2.WorldPosition))
        .Then(|| this.LookAtPlane(target3.WorldPosition))
        .Then(|| this.MoveTowards(target3.WorldPosition, moveSpeed))
        .Until(Condition.WaitForLinearMove(this.GameObject, target3.WorldPosition))
        .Then(function() actionsFinished = true end)
        .ThenFinish()
end