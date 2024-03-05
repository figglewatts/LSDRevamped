require "dreams"

player = GetEntity("__player")
target = GetEntity("BlimpTarget").WorldPosition

moveSpeed = 0.3

function start()
    this.SetChildVisible(false)
    
    if IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end
end

function interact()
    this.LogGraphContribution(-6, 7)
    this.SetChildVisible(true)

    if Random.OneIn(4) then
        DreamSystem.StretchDream(2, 1)
    end

    if Random.OneIn(2) then
        this.Action
            .Do(|| this.MoveTowards(target, moveSpeed))
            .Until(Condition.WaitForLinearMove(this.GameObject, target))
            .Then(|| moveDown())
            .Until(Condition.WaitForSeconds(15))
            .Then(|| link())
            .ThenFinish()
    else
        this.Action
            .Do(|| this.MoveInDirection(this.Forward, moveSpeed))
            .Until(Condition.WaitForSeconds(60))
            .Then(|| this.GameObject.SetActive(false))
            .ThenFinish()
    end
end

function moveDown()
    this.MoveInDirection(this.Up.negated(), moveSpeed)
    this.LookTowards(this.GameObject.WorldPosition - this.GameObject.RightDirection, 5)
end

function link()
    DreamSystem.SetNextTransitionDream(dreams.ClockworkMachines)
    DreamSystem.TransitionToDream()
end