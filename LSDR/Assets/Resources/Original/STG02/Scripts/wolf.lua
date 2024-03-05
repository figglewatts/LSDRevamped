player = GetEntity("__player")

moveSpeed = 0.7
interacted = false
turned = false

function start()
    this.SetChildVisible(false)
    
    if not IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end
end

function update()
    if not interacted then return end

    local speed = moveSpeed
    if turned then speed = speed * 5 end
    this.MoveInDirection(this.Forward, speed)
end

function interact()
    interacted = true
    this.SetChildVisible(true)

    direction = Unity.Vector3(1, 0, 0)
    if Random.OneIn(2) then direction = direction.negated() end

    this.LogGraphContribution(-3, 2)

    this.Action
        .Do(|| this.PlayAnimation(0))
        .ThenWaitUntil(Condition.WaitForSeconds(5))
        .Then(|| this.LookInDirection(direction))
        .Then(function() turned = true end)
        .ThenFinish()
end