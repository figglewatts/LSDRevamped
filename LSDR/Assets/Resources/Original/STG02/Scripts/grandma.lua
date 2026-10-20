player = GetEntity("__player")

stretches = false
stretched = false
distanceToPlayer = 0

function start()
    this.PlayAnimation(0)
    
    if not IsDayEven() or (this.GameObject.Name ~= "Grandma" and Random.OneIn(2)) then
        this.GameObject.SetActive(false)
        return
    end

    stretches = Random.OneIn(2)
end

function intervalUpdate()
    distanceToPlayer = (player.WorldPosition - this.GameObject.WorldPosition).length()
    this.LookAtPlane(player.WorldPosition)
end

function update()
    if not stretches or stretched or distanceToPlayer > 1.25 then return end

    stretched = true
    DreamSystem.StretchDream(2, 0.2)
    this.Action
        .WaitUntil(Condition.WaitForSeconds(15))
        .Then(|| DreamSystem.EndDream())
        .ThenFinish()
end

function interact()
    this.LogGraphContribution(-3, -5)
end