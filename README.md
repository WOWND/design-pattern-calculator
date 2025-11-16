# 디자인 패턴 계산기



### 주요 기능

-  기본 연산 (덧셈, 뺄셈, 곱셈, 나눗셈)
-  고급 연산 (거듭제곱, 나머지)
-  단항 연산 (sin, cos, tan, sqrt, log, ln, abs)
-  괄호를 사용한 복잡한 수식 지원
-  수식 표시 및 계산
-  음수 지원 (단항 마이너스)
-  계산 이력 관리
-  실행 취소/다시 실행 (Undo/Redo)
-  계산 결과 캐싱
-  계산 과정 로깅
-  초기화 기능
-  GUI 지원



### 프로젝트 구조

1. **DesignPatternCalculator**
   - **Core**: 계산기 핵심 기능, 인터페이스, 캐시
   - **Expressions**: 수식 관련 기능들
   - **Operations**: 연산 관련 기능들
   - **Calculator**: 퍼사드 패턴을 통한 간단한 인터페이스
   - **Memento**: 실행 취소/다시 실행을 위한 상태 저장
2. **DesignPatternCalculator.ConsoleUI**: 콘솔 기반 UI
3. **DesignPatternCalculator.Avalonia**: Avalonia기반 GUI

## 적용된 디자인 패턴

#### 팩토리 메서드 패턴

**목적**: 연산자 기호에 따라 적절한 연산 객체를 생성합니다.

**적용 방법**:
- `IOperationFactory` 인터페이스를 정의하여 연산 객체 생성을 추상화
- `OperationFactory` 클래스에서 연산자 문자열을 기반으로 구체적인 연산 객체 생성
- Dictionary를 사용하여 연산자와 생성 함수를 매핑

---


#### 싱글턴 패턴

**목적**: 계산기 엔진의 인스턴스를 하나만 유지하여 전역적으로 접근 가능하게 합니다.

**적용 방법**:
- 스레드 안전 초기화 구현
- private 생성자로 외부 인스턴스 생성 차단
- static Instance 프로퍼티로 전역 접근점 제공

---

#### 빌더 패턴

**목적**: 복잡한 수식을 단계적으로 구성합니다.

**적용 방법**:
- `IExpressionBuilder` 인터페이스 정의
- 연산자 우선순위를 고려한 수식 트리 구성

---

#### 복합체 패턴 (컴포지트)

**목적**: 수식을 트리 구조로 표현하여 부분-전체 계층을 구성합니다.

**적용 방법**:
- `IExpression` 인터페이스를 Component로 정의
- `NumberExpression`을 Leaf로 구현 (단말 노드)
- `BinaryOperationExpression`과 `UnaryOperationExpression`을 Composite로 구현

---

#### 데코레이터 패턴

**목적**: 연산 객체에 동적으로 추가 기능을 부여합니다.

**적용 방법**:
- `OperationDecorator` 추상 클래스로 기본 데코레이터 정의
- `LoggingOperationDecorator`로 로깅 기능 추가
- `CachingCalculatorDecorator`로 캐싱 기능 추가

---

#### 퍼사드 패턴

**목적**: 복잡한 서브시스템을 간단한 인터페이스로 제공합니다.

**적용 방법**:
- Builder, Factory, Singleton, Memento 등 여러 패턴을 내부에서 사용
- 사용자에게는 간단한 메서드만 노출 (CalculateExpression, Undo, Redo 등)
- 복잡한 내부 구현을 숨김

---

#### 메멘토 패턴

**목적**: 계산 상태를 저장하고 복원하여 실행 취소/다시 실행 기능을 제공합니다.

**적용 방법**:
- `CalculatorMemento`로 수식과 결과 상태 캡슐화
- `CalculatorCaretaker`로 메멘토 히스토리 관리
- Undo/Redo 스택을 통한 상태 탐색

---

#### 어댑터 패턴

**목적**: 기존 수학 라이브러리를 계산기 인터페이스에 맞게 변환합니다.

**적용 방법**:
- `IUnaryOperation` 인터페이스를 통해 다양한 수학 함수 통합
- Math 클래스의 정적 메서드를 래핑하여 일관된 인터페이스 제공
- 삼각함수에서 도(degree)를 라디안(radian)으로 자동 변환

