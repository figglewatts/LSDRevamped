require "dreams"

player = GetEntity("__player")

moveSpeed = 0.5
riseSpeed = 0.6
tumbling = false
linked = false
distanceToPlayer = 0

function start()
    this.PlayAnimation(0)

    -- the second astronaut only turns up some of the time
    local extra = this.GameObject.Name ~= "Astronaut"
    if not IsDayEven() or (extra and Random.OneIn(2)) then
        this.GameObject.SetActive(false)
        return
    end

    if ChartZone() == "magenta" then
        floatAway()
    else
        pace()
    end
end

function pace()
    local away = Unity.Vector3(-1, 0, 0)
    this.Action
        .Do(|| this.MoveInDirection(away, moveSpeed))
        .Until(Condition.WaitForSeconds(60))
        .Then(|| this.MoveInDirection(away.negated(), moveSpeed))
        .Until(Condition.WaitForSeconds(60))
        .ThenFinish()
end

function floatAway()
    local away = Unity.Vector3(-1, 0, 0)
    local rise = Unity.Vector3(1, 1, 0)
    this.Action
        .Do(|| this.MoveInDirection(away, moveSpeed))
        .Until(Condition.WaitForSeconds(5))
        .Then(|| this.MoveInDirection(rise, riseSpeed))
        .Until(Condition.WaitForSeconds(7.5))
        .Then(|| this.StopAnimation())
        .ThenWaitUntil(Condition.WaitForSeconds(0.5))
        .Then(function()
            this.MoveInDirection(away.negated(), moveSpeed)
            this.Rotate(Unity.Vector3(0, -210, 0))
        end)
        .Until(Condition.WaitForSeconds(15))
        .Then(function() tumbling = true end)
        .ThenFinish()
end

function intervalUpdate()
    distanceToPlayer = (player.WorldPosition - this.GameObject.WorldPosition).length()
end

function update()
    if tumbling then
        this.Rotate(Unity.Vector3(90, -210, 50))
    end

    if distanceToPlayer < 1 and not linked then
        linked = true
        DreamSystem.SetNextTransitionDream(dreams.Void)
        DreamSystem.TransitionToDream()
    end
end

function interact()
    this.LogGraphContribution(0, 2, this)
end