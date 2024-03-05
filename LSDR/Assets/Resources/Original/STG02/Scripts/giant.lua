moveSpeed = 0.4

function start()
    this.PlayAnimation(0)
    
    if not IsDayEven() or Random.OneIn(2) then
        this.GameObject.SetActive(false)
        return
    end
end

function update()
    this.MoveInDirection(this.GameObject.ForwardDirection.negated(), moveSpeed)
end

function interact()
    this.LogGraphContribution(-7, 3)
end