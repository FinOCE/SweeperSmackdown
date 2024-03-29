import React, { ChangeEvent, useCallback } from "react"

type SliderInputProps = {
  id: string
  name: string
  value: number
  min: number
  max: number
  step?: number
  onChange: (e: number) => void
  disabled?: boolean
  display?: (value: number) => string
}

export function SliderInput(props: SliderInputProps) {
  const set = useCallback((e: ChangeEvent<HTMLInputElement>) => props.onChange(Number(e.target.value)), [])

  function increment() {
    props.onChange(props.value + (props.step ?? 1))
  }

  function decrement() {
    props.onChange(props.value - (props.step ?? 1))
  }

  return (
    <div>
      <input type="button" value="-" onClick={decrement} disabled={props.disabled || props.value <= props.min} />
      <input
        type="range"
        name={props.name}
        id={props.id}
        min={props.min}
        max={props.max}
        step={props.step}
        value={props.value}
        onChange={set}
        disabled={props.disabled}
      />
      <input type="button" value="+" onClick={increment} disabled={props.disabled || props.value >= props.max} />
      <output htmlFor={props.id}>{props.display ? props.display(props.value) : props.value}</output>
    </div>
  )
}
