moveSpeed = 0.4

function start()
    if not IsDayEven() or Random.OneIn(2) then
        this.GameObject.SetActive(false)
        return
    end

    this.PlayAnimation(0)
end

function update()
    this.MoveInDirection(this.GameObject.ForwardDirection.negated(), moveSpeed)
end

function interact()
    this.LogGraphContribution(-7, 3)
end