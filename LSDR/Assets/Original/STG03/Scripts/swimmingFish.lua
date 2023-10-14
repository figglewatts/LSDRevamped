fishObj = this.GameObject.GetChildByName("0")

t = 0

function update()
    t = t + Unity.DeltaTime()
    local sinT = math.sin(t * 4)
    local cosT = math.cos(t * 4)
    local posMod = Unity.Vector3(sinT / 8, cosT / 8, 0)
    fishObj.LocalPosition = posMod
end

function interact()
    DreamSystem.LogGraphContributionFromEntity(6, 0)
    this.PlayAnimation(0)
    this.Action
        .Do(|| this.MoveInDirection(this.Forward, 1.5))
        .Until(Condition.WaitForSeconds(2))
        .Then(|| this.MoveInDirection(this.Forward.negated(), 1.5))
        .Until(Condition.WaitForSeconds(2))
        .ThenLoop()
end